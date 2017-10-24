//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;

namespace MaterialUI
{
    /// <summary>
    /// Base class for a Text validator.
    /// </summary>
    public class BaseTextValidator
	{
        /// <summary>
        /// The material input field.
        /// </summary>
        protected MaterialInputField m_MaterialInputField;

        /// <summary>
        /// Initializes the validator for the specified material input field.
        /// </summary>
        /// <param name="materialInputField">The material input field.</param>
        public void Init(MaterialInputField materialInputField)
		{
			m_MaterialInputField = materialInputField;
		}
	}

    /// <summary>
    /// Text validator that checks for empty text.
    /// </summary>
    /// <seealso cref="MaterialUI.BaseTextValidator" />
    /// <seealso cref="MaterialUI.ITextValidator" />
    public class EmptyTextValidator : BaseTextValidator, ITextValidator
	{
		/// <summary>
		/// The error message.
		/// </summary>
		private string m_ErrorMessage;

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialUI.EmptyTextValidator"/> class.
		/// </summary>
		/// <param name="errorMessage">Error message.</param>
		public EmptyTextValidator(string errorMessage = "Can't be empty")
		{
			m_ErrorMessage = errorMessage;
		}

        /// <summary>
        /// Determines whether the text is valid.
        /// </summary>
        /// <returns>True if the text contains characters, otherwise false.</returns>
        public bool IsTextValid()
		{
			if (string.IsNullOrEmpty(m_MaterialInputField.inputField.text))
			{
				m_MaterialInputField.validationText.text = m_ErrorMessage;
				return false;
			}

			return true;
		}
	}

    /// <summary>
    /// Text validator that checks for emails.
    /// </summary>
    /// <seealso cref="MaterialUI.BaseTextValidator" />
    /// <seealso cref="MaterialUI.ITextValidator" />
    public class EmailTextValidator : BaseTextValidator, ITextValidator
	{
		/// <summary>
		/// The error message.
		/// </summary>
		private string m_ErrorMessage;

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialUI.EmailTextValidator"/> class with a custom error message.
		/// </summary>
		/// <param name="errorMessage">Error message.</param>
		public EmailTextValidator(string errorMessage = "Format is invalid")
		{
			m_ErrorMessage = errorMessage;
		}

        /// <summary>
        /// Determines whether the text is valid.
        /// </summary>
        /// <returns>True if the text appears to be an email, otherwise false.</returns>
        public bool IsTextValid()
		{
			Regex regex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");
			Match match = regex.Match(m_MaterialInputField.inputField.text);
			if (!match.Success)
			{
				m_MaterialInputField.validationText.text = m_ErrorMessage;
				return false;
			}

			return true;
		}
	}

    /// <summary>
    /// Text validator that checks for names.
    /// </summary>
    /// <seealso cref="MaterialUI.BaseTextValidator" />
    /// <seealso cref="MaterialUI.ITextValidator" />
    public class NameTextValidator : BaseTextValidator, ITextValidator
	{
		/// <summary>
		/// The error message.
		/// </summary>
		private string m_ErrorMessage;

        /// <summary>
        /// The minimum length.
        /// </summary>
        private int m_MinimumLength = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="NameTextValidator"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		public NameTextValidator(string errorMessage = "Format is invalid") 
		{
			m_ErrorMessage = errorMessage;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="NameTextValidator"/> class.
        /// </summary>
		/// <param name="minimumLength">The minimum length.</param>
		/// <param name="errorMessage">The error message.</param>
		public NameTextValidator(int minimumLength, string errorMessage = "Format is invalid")
		{
			m_MinimumLength = minimumLength;
			m_ErrorMessage = errorMessage;
		}

        /// <summary>
        /// Determines whether the text is valid.
        /// </summary>
        /// <returns>True if the text appears to be a name, otherwise false.</returns>
        public bool IsTextValid()
		{
			if (m_MaterialInputField.inputField.text.Length < m_MinimumLength)
			{
				m_MaterialInputField.validationText.text = m_ErrorMessage;
				return false;
			}

			Regex regex = new Regex(@"^\p{L}+(\s+\p{L}+)*$");
			Match match = regex.Match(m_MaterialInputField.inputField.text);
			if (!match.Success)
			{
				m_MaterialInputField.validationText.text = m_ErrorMessage;
				return false;
			}

			return true;
		}
	}

    /// <summary>
    /// Text validator that checks for length.
    /// </summary>
    /// <seealso cref="MaterialUI.BaseTextValidator" />
    /// <seealso cref="MaterialUI.ITextValidator" />
    public class LengthTextValidator : BaseTextValidator, ITextValidator
	{
		/// <summary>
		/// The error message.
		/// </summary>
		private string m_ErrorMessage;

        /// <summary>
        /// The minimum length
        /// </summary>
        private int m_MinimumLength = 6;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordTextValidator"/> class.
        /// </summary>
        /// <param name="minimumLength">The minimum length.</param>
		/// <param name="errorMessage">The error message.</param>
		public LengthTextValidator(int minimumLength, string errorMessage)
		{
			m_MinimumLength = minimumLength;

			if (string.IsNullOrEmpty(errorMessage))
			{
				m_ErrorMessage = "Require at least " + m_MinimumLength + " characters";
			}
			else
			{
				m_ErrorMessage = errorMessage;
			}
		}

        /// <summary>
        /// Determines whether the text is valid.
        /// </summary>
        /// <returns>True if the text is at least the length of or longer than m_MinimumLength, otherwise false.</returns>
        public bool IsTextValid()
		{
			if (m_MaterialInputField.inputField.text.Length < m_MinimumLength)
			{
				m_MaterialInputField.validationText.text = m_ErrorMessage;
				return false;
			}

			return true;
		}
	}

    /// <summary>
    /// Text validator that checks two passwords to see if they are the same.
    /// </summary>
    /// <seealso cref="MaterialUI.BaseTextValidator" />
    /// <seealso cref="MaterialUI.ITextValidator" />
    public class SamePasswordTextValidator : BaseTextValidator, ITextValidator
	{
		/// <summary>
		/// The error message.
		/// </summary>
		private string m_ErrorMessage;

        /// <summary>
        /// The original password input field
        /// </summary>
        private InputField m_OriginalPasswordInputField;

        /// <summary>
        /// Initializes a new instance of the <see cref="SamePasswordTextValidator"/> class.
        /// </summary>
		/// <param name="originalPasswordInputField">The original password input field.</param>
		/// <param name="errorMessage">The error message.</param>
		public SamePasswordTextValidator(InputField originalPasswordInputField, string errorMessage = "Passwords are different!")
		{
			m_OriginalPasswordInputField = originalPasswordInputField;
			m_ErrorMessage = errorMessage;
		}

        /// <summary>
        /// Determines whether the text is valid.
        /// </summary>
        /// <returns>True if the text in the two fields match, otherwise false.</returns>
        public bool IsTextValid()
		{
			if (!m_MaterialInputField.inputField.text.Equals(m_OriginalPasswordInputField.text))
			{
				m_MaterialInputField.validationText.text = m_ErrorMessage;
				return false;
			}

			return true;
		}
	}

    /// <summary>
    /// Text validator that checks for existing directories.
    /// </summary>
    /// <seealso cref="MaterialUI.BaseTextValidator" />
    /// <seealso cref="MaterialUI.ITextValidator" />
    public class DirectoryExistTextValidator : BaseTextValidator, ITextValidator
	{
		/// <summary>
		/// The error message.
		/// </summary>
		private string m_ErrorMessage;

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialUI.DirectoryExistTextValidator"/> class.
		/// </summary>
		/// <param name="errorMessage">Error message.</param>
		public DirectoryExistTextValidator(string errorMessage = "This directory does not exist")
		{
			m_ErrorMessage = errorMessage;
		}

        /// <summary>
        /// Determines whether the text is valid.
        /// </summary>
        /// <returns>True if the directory given by the text value exists, otherwise false.</returns>
        public bool IsTextValid()
		{
			bool directoryExists = Directory.Exists(m_MaterialInputField.inputField.text);
			if (!directoryExists)
			{
				m_MaterialInputField.validationText.text = m_ErrorMessage;
			}

			return directoryExists;
		}
	}

    /// <summary>
    /// Text validator that checks for existing files.
    /// </summary>
    /// <seealso cref="MaterialUI.BaseTextValidator" />
    /// <seealso cref="MaterialUI.ITextValidator" />
    public class FileExistTextValidator : BaseTextValidator, ITextValidator
	{
		/// <summary>
		/// The error message.
		/// </summary>
		private string m_ErrorMessage;

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialUI.DirectoryExistTextValidator"/> class.
		/// </summary>
		/// <param name="errorMessage">Error message.</param>
		public FileExistTextValidator(string errorMessage = "This file does not exist")
		{
			m_ErrorMessage = errorMessage;
		}

        /// <summary>
        /// Determines whether the text is valid.
        /// </summary>
        /// <returns>True if the filepath given by the text value exists, otherwise false.</returns>
        public bool IsTextValid()
		{
			bool fileExists = File.Exists(m_MaterialInputField.inputField.text);
			if (!fileExists)
			{
				m_MaterialInputField.validationText.text = m_ErrorMessage;
			}

			return fileExists;
		}
	}

}