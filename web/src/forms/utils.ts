import { MAX_SELECT_YEAR, MIN_SELECT_YEAR } from '@/constants/date';
import { getPleaseEnterAValidFieldErrorMessage } from '@/constants/errors';
import {
	LETTERS_AND_NUMBERS_REG_EXP,
	ONLY_PUNCTUATION_OR_SPACES_EXP,
	ONLY_SPACES,
} from '@/constants/regexp';
import { DateRangeValidator, PlateNode } from '@/forms/types';
import { extractPlainText } from '@/helpers/plain';
import dayjs from '@/lib/dayjs';

const SECOND_SYMBOL_INDEX = 1;
const DEFAULT_MAX_DATE_RANGE_YEARS = 1;

export const checkToAllSpaces = (value: string | undefined | null) =>
	!(value?.split('').every((char) => char === ' ') && value?.split('').length);

export const calculateLengthWithoutCommas = (value: string | undefined) => {
	const valueWithoutCommas = value ? value.replace(/,/g, '') : '';

	return valueWithoutCommas.length;
};

export const checkToAllSpacesFromSecondSymbol = (value: string | undefined) =>
	value && value.length > SECOND_SYMBOL_INDEX
		? value
				.slice(SECOND_SYMBOL_INDEX)
				.split('')
				.every((char) => char === ' ')
		: false;

export const checkIfNoLettersOrNumbers = (value: string | undefined | null) =>
	value ? !value.match(LETTERS_AND_NUMBERS_REG_EXP) : true;

export const checkToAllHyphens = (value: string | undefined | null) =>
	!(value?.split('').every((char) => char === '-') && value?.split('').length);

export const checkToAllHyphensAndSpaces = (value: string | undefined | null) =>
	!(
		value?.split('').every((char) => char === '-' || char === ' ') &&
		value?.split('').length
	);

export const checkToMultipleSpacesAtOnce = (value: string | undefined | null) =>
	!value?.includes('  ');

export const checkNumberLength = (limit: number, value?: string) => {
	if (value !== undefined) {
		return value.toString().length <= limit;
	}

	return true;
};

export const checkYearInValidRange = (date?: string | null) => {
	if (!date) {
		return true;
	}

	const yearStartIndex = 11; // start year index in strings like 'Mon 01 Jan 2200 00:00:00 GMT'

	// Date.parse(), new Date(), .... return formatted year(0022=>2022) that is not what we expect
	const year = Number(date.slice(yearStartIndex, yearStartIndex + 4));

	return year <= MAX_SELECT_YEAR && year >= MIN_SELECT_YEAR;
};

export const checkUrlToAvatar = (value?: string) => {
	if (!value) {
		return false;
	}

	if (!(value.startsWith('http://') || value.startsWith('https://'))) {
		return false;
	}

	if (!['.jpg', '.jpeg', '.png'].some((item) => value.endsWith(item))) {
		return false;
	}

	try {
		(() => new URL(value))();
	} catch {
		return false;
	}

	return true;
};

export const checkEmptyField = (value?: string | null) => Boolean(value);

export const checkIncludesOtherThanSpecialSymbols = (value?: string) =>
	!ONLY_PUNCTUATION_OR_SPACES_EXP.test(value!);

export const checkIfThereIsNoEnoughChars = (
	counter: number,
	str?: string | null
) =>
	str ? str.trim().replace(ONLY_SPACES, ' ').split('').length >= counter : true;

export const findDepartmentById = <T extends { id: string }>(
	options: T[],
	depId: string
) => options.find((department) => department.id === depId);

export const getNotValidFieldMessage = (fieldName: string) => {
	const TEMPLATE = getPleaseEnterAValidFieldErrorMessage(fieldName);

	return fieldName.toLowerCase() === 'notes'
		? TEMPLATE.replace(' a ', ' ')
		: TEMPLATE;
};

export const getPlainText = (nodes: PlateNode[]): string => {
	const parsedNodes = typeof nodes === 'string' ? JSON.parse(nodes) : nodes;

	if (!Array.isArray(parsedNodes)) {
		return '';
	}

	const rawText = extractPlainText(parsedNodes);

	return rawText;
};

export const validateDateRangeWithinMaxYears =
	(years = DEFAULT_MAX_DATE_RANGE_YEARS): DateRangeValidator =>
	(fromDate, toDate) =>
		dayjs(toDate)
			.startOf('day')
			.isSameOrBefore(dayjs(fromDate).startOf('day').add(years, 'year'));
