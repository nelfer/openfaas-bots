using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Function
{
	public class Crypto
	{
		#region Member Variables
		public readonly static byte[] GlobalEK = Encoding.UTF8.GetBytes("NGCKEY4b49441b315c30b7c5");
		readonly static byte[] _GlobalIV = Encoding.UTF8.GetBytes("48929389");
		#endregion

		#region Properties
		byte[] _EncryptionKey;
		public byte[] EncryptionKey { get { return _EncryptionKey; } set { _EncryptionKey = value; } }

		byte[] _IV;
		public byte[] IV { get { return _IV; } set { _IV = value; } }

		bool _IsUseURLEncoding = false;
		public bool IsUseURLEncoding { get { return _IsUseURLEncoding; } set { _IsUseURLEncoding = value; } }
		#endregion

		#region Public Methods
		public string Encrypt(string _toEncrypt)
		{
			return Encrypt(_toEncrypt, (_EncryptionKey == null ? GlobalEK : _EncryptionKey), (_IV == null ? _GlobalIV : _IV));
		}

		public string Decrypt(string _toDecrypt)
		{
			return Decrypt(_toDecrypt, (_EncryptionKey == null ? GlobalEK : _EncryptionKey), (_IV == null ? _GlobalIV : _IV));
		}
		#endregion

		public byte[] HMACSHA256(string _toSign,byte[] _key)
		{
			byte[] hash;
			using(HMACSHA256 hmac = new HMACSHA256(_key))
			{
				hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(_toSign));
			}
			return hash;
		}

		#region Private Methods
		#region Encrypt
		string Encrypt(string _toEncrypt, byte[] _encryptionKey)
		{
			return Encrypt(_toEncrypt, _encryptionKey, _GlobalIV);
		}

		string Encrypt(string _toEncrypt, byte[] _encryptionKey, byte[] _IV)
		{
			string encryptedString = "";
			try
			{
				MemoryStream mStream = new MemoryStream();

				CryptoStream cStream = new CryptoStream(mStream, new TripleDESCryptoServiceProvider().CreateEncryptor(_encryptionKey, _IV), CryptoStreamMode.Write);
				byte[] Data = Encoding.UTF8.GetBytes(_toEncrypt);

				cStream.Write(Data, 0, Data.Length);
				cStream.FlushFinalBlock();

				byte[] encryptedData = mStream.ToArray();

				cStream.Close();
				mStream.Close();

				encryptedString = Convert.ToBase64String(encryptedData);

				if(IsUseURLEncoding)
					encryptedString = Base64ForUrlEncode(encryptedString);
			}
			catch
			{
				throw;
			}
			return encryptedString;
		}
		#endregion

		#region Decrypt
		string Decrypt(string _toDecrypt, byte[] _encryptionKey)
		{
			return Decrypt(_toDecrypt, _encryptionKey, _GlobalIV);
		}

		string Decrypt(string _toDecrypt, byte[] _encryptionKey, byte[] _IV)
		{
			string decryptedString = "", encryptedStr = "";
			try
			{
				encryptedStr = IsUseURLEncoding ? Base64ForUrlDecode(_toDecrypt) : _toDecrypt;

				byte[] data = Convert.FromBase64String(encryptedStr);
				MemoryStream msDecrypt = new MemoryStream(data);

				CryptoStream csDecrypt = new CryptoStream(msDecrypt, new TripleDESCryptoServiceProvider().CreateDecryptor(_encryptionKey, _IV), CryptoStreamMode.Read);

				byte[] fromEncrypt = new byte[data.Length];

				csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

				decryptedString = Encoding.UTF8.GetString(fromEncrypt).TrimEnd('\0');
			}
			catch
			{
				throw;
			}
			return decryptedString;
		}
		#endregion
		
		#region URLEncryption/Decryption Methods
		public string Base64ForUrlEncode(byte[] _encbuff)
		{
			return Convert.ToBase64String(_encbuff).Replace('+', '-').Replace('/', '_').Replace("=", "");
		}

		public string Base64ForUrlEncode(string _str)
		{
			byte[] encbuff = Encoding.UTF8.GetBytes(_str);
			return Convert.ToBase64String(encbuff).Replace('+', '-').Replace('/', '_').Replace("=", "");
		}

		public string Base64ForUrlDecode(string _strb64)
		{
			_strb64=_strb64.Replace('-', '+').Replace('_', '/');
			_strb64 = _strb64.PadRight(_strb64.Length + (4 - _strb64.Length % 4) % 4, '=');

			return Encoding.UTF8.GetString(Convert.FromBase64String(_strb64));
		}

		string UrlTokenEncode(byte[] _strByte)
		{
			if(_strByte == null)
				throw new ArgumentNullException("_strByte");

			string encodedValue = "";

			if(_strByte.Length > 0)
			{

				string base64 = null;
				int endPos = 0;
				char[] temp = null;

				base64 = Convert.ToBase64String(_strByte);
				if(base64 == null)
					return null;

				for(endPos = base64.Length; endPos > 0; endPos--)
				{
					if(base64[endPos - 1] != '=')
					{
						break;
					}
				}

				temp = new char[endPos + 1];
				temp[endPos] = (char)((int)'0' + base64.Length - endPos);

				for(int i = 0; i < endPos; i++)
				{
					char c = base64[i];

					switch(c)
					{
						case '+':
							temp[i] = '-';
							break;
						case '/':
							temp[i] = '_';
							break;
						case '=':
							temp[i] = c;
							break;
						default:
							temp[i] = c;
							break;
					}
				}
				encodedValue = new string(temp);
			}

			return encodedValue;
		}

		byte[] UrlTokenDecode(string _str)
		{
			if(_str == null) { throw new ArgumentNullException("_str"); }

			byte[] decoded = null;
			int len = _str.Length;

			if(len > 0)
			{
				int numPad = (int)_str[len - 1] - (int)'0';

				if(numPad >= 0 && numPad <= 10)
				{
					char[] base64 = new char[len - 1 + numPad];

					for(int i = 0; i < len - 1; i++)
					{
						char c = _str[i];

						switch(c)
						{
							case '-':
								base64[i] = '+';
								break;
							case '_':
								base64[i] = '/';
								break;
							default:
								base64[i] = c;
								break;
						}
					}

					for(int i = len - 1; i < base64.Length; i++)
					{
						base64[i] = '=';
					}

					decoded = Convert.FromBase64CharArray(base64, 0, base64.Length);
				}
			}
			else
			{
				decoded = new byte[0];
			}

			return decoded;
		}
		#endregion
		#endregion
	}
}