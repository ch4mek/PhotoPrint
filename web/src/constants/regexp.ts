export const EMAIL_REG_EXP = /^([a-zA-Z0-9.\-_]+@[^@]*)$/;
export const MODSEN_DOMAIN_REG_EXP = /@modsen-software\.com$/;

export const EMAIL_REG_EXP_DOMAIN =
	/^([a-zA-Z\d._%+-]+)@([a-zA-Z\d-]+\.[a-zA-Z\d]{2,})(?:\.[a-zA-Z\d]{2,})?$/;
export const PHONE_REG_EXP = /^([+][\s0-9]+?)$/gm;
export const LETTERS_AND_NUMBERS_REG_EXP = /[a-zA-Zа-яА-яёЁ\d]/;
export const ONLY_PUNCTUATION_OR_SPACES_EXP =
	/^[!#№@"$;%:^?&*()_=+~`{}[\]\\|/"':;><.,\s-]+$/;
export const ONLY_DIGITS_REG_EXP = /^\d+$/;
export const ONLY_DIGITS_WITH_EMPTY_VALUE_REG_EXP = /^\d*$/;
export const ONLY_POSITIVE_DIGITS_REG_EXP = /^[1-9]\d*$/;
export const ONLY_POSITIVE_DIGITS_AND_ZERO_REG_EXP = /^[0-9]\d*$/;
export const ONLY_DIGITS_AND_PLUS_REG_EXP = /[^\d+]/g;
// Might be reused in other schemas like Numbers_type_2
// export const FRACTIONS_AND_PLUS_REG_EXP = /^\+?\d+(,\d{3})*(\.\d{1,2})?$/;
export const CURRENCY_REG_EXP =
	/^([$€]?\d{1,3}(?:,\d{3})*(\.\d{1,2})?)?(zł|р)?$/;
export const TIME_REG_EXP = /^(2[0-3]|[01][0-9]):([0-5][0-9])$/;
export const DATE_REG_EXP =
	/^(0[1-9]|[12][0-9]|3[01])\.(0[1-9]|1[0-2])\.(?!0000)\d{4}$/;
export const CONTENT_DISPOSITION_FILE_NAME_REG_EXP = /filename=([^;]*)/;
export const ONLY_SPACES = /\s+/g;
export const WORDS_BY_UPPERCASE_OR_LOWERCASE_LETTER = /(?=[A-Z])(?<=[a-z])|_/g;
export const getDigitsWithMaxLengthRegExp = (maxLength: number) =>
	new RegExp(`^\\d{1,${maxLength}}$`);
export const getDigitsRegExp = () => /^\d+$/;
