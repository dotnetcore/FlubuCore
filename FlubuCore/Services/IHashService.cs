using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Services
{
	public interface IHashService
	{
		string Hash(string valueToHash);

		bool Validate(string valueToValidate, string correctHash);
	}
}
