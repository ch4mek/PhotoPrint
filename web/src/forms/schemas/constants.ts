const getRegExpString = (value: string) => value.replace(/\//g, '');

const LETTERS = /a-zA-Zа-яА-яёЁ/;
const RUSSIAN_LETTERS = /а-яА-яёЁ/;
const DIGITS = /0-9/;
const HYPHENS_AND_SPACE = /-\s/;

const PUNCTUATIONS = /.,:;_'`/;
const BRACKETS = /\(\)\[\]<>{}/;
const SPECIAL_SYMBOLS = /\+@#/;

const TEXT_TYPE_1 = getRegExpString(`${LETTERS}${HYPHENS_AND_SPACE}`);
const TEXT_TYPE_1_2 = getRegExpString(`${RUSSIAN_LETTERS}${HYPHENS_AND_SPACE}`);
const TEXT_TYPE_2 = getRegExpString(`${TEXT_TYPE_1}${PUNCTUATIONS}${BRACKETS}`);
const TEXT_TYPE_3 = getRegExpString(
	`${TEXT_TYPE_2}${SPECIAL_SYMBOLS}${DIGITS}`
);

export const TEXT_TYPE_1_REG_EXP = new RegExp(`^[${TEXT_TYPE_1}]+$`, 'g');
export const TEXT_TYPE_1_2_REG_EXP = new RegExp(`^[${TEXT_TYPE_1_2}]+$`, 'g');
export const TEXT_TYPE_2_REG_EXP = new RegExp(
	`^[${TEXT_TYPE_2}/&\\\\|]+$`,
	'g'
);
export const TEXT_TYPE_3_REG_EXP = new RegExp(`^[${TEXT_TYPE_3}]+$`, 'g');

export const MAX_CHARACTERS = 8;
const DATE_SEPARATORS_COUNTER = 2;

export const DATE_TYPE_1_MAX_CHARACTERS =
	MAX_CHARACTERS + DATE_SEPARATORS_COUNTER;

export const MAX_NUMBER_TYPE_1_DIGITS = 4;
