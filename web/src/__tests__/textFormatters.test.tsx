import { TFunction } from 'i18next';

import { TextWithStrongInside } from '@/components/TextWithStrongInside';
import { checkIsTextCut } from '@/helpers/checkIsTextCut';
import { formatDiscountPercent } from '@/helpers/formatDiscountPercent';
import { formatToFixedPrecision } from '@/helpers/formatToFixedPrecision';
import { getDialogTextComponent } from '@/helpers/getDeleteDialogTextComponent';
import { getIdFromPathname } from '@/helpers/getIdFromPathname';
import { getSeniorityAndSpecialization } from '@/helpers/getSeniorityAndSpecialization';
import { getTrimedString } from '@/helpers/getTrimedString';
import { getTrimedUserUpdateItem } from '@/helpers/getTrimedUserUpdateItem';
import { lowercaseFirstSymbol } from '@/helpers/lowercaseFirstSymbol';
import { mapNamesFromArray } from '@/helpers/mapNamesFromArray';
import {
	SetFieldValue,
	setNumbersType1Value,
} from '@/helpers/setNumbersType1Value';
import { isStringArray } from '@/helpers/typePredicates';
import { UseUpdateUserResponse } from '@/types/userInfo';

jest.mock('@/components/TextWithStrongInside');

const mockBookingT = ((key: string) => key) as TFunction<'booking'>;

describe('Tests for text formating utils', () => {
	describe('getSeniorityAndSpecialization', () => {
		it('should return formatted string when both parameters provided', () => {
			expect(getSeniorityAndSpecialization('Senior', 'Frontend')).toBe(
				'Senior (Frontend)'
			);
		});

		it('should return dash when seniority is null', () => {
			expect(getSeniorityAndSpecialization(null, 'Frontend')).toBe('-');
		});

		it('should return dash when specialization is null', () => {
			expect(getSeniorityAndSpecialization('Senior', null)).toBe('-');
		});

		it('should return dash when both parameters are null', () => {
			expect(getSeniorityAndSpecialization(null, null)).toBe('-');
		});

		it('should return dash when seniority is empty string', () => {
			expect(getSeniorityAndSpecialization('', 'Frontend')).toBe('-');
		});

		it('should return dash when specialization is empty string', () => {
			expect(getSeniorityAndSpecialization('Senior', '')).toBe('-');
		});
	});

	describe('getTrimedUserUpdateItem', () => {
		const mockUser = {
			id: '1',
			firstName: 'John',
			lastName: 'Doe',
			email: 'john@example.com',
		} as unknown as UseUpdateUserResponse;

		it('should trim firstName and lastName', () => {
			const input = { ...mockUser, firstName: '  John  ', lastName: '  Doe  ' };
			const result = getTrimedUserUpdateItem(input);

			expect(result.firstName).toBe('John');
			expect(result.lastName).toBe('Doe');
		});

		it('should preserve other properties', () => {
			const input = { ...mockUser, firstName: '  John  ', lastName: '  Doe  ' };
			const result = getTrimedUserUpdateItem(input);

			expect(result.id).toBe('1');
			expect(result.email).toBe('john@example.com');
		});

		it('should not mutate original object', () => {
			const input = { ...mockUser, firstName: '  John  ', lastName: '  Doe  ' };
			const original = { ...input };
			getTrimedUserUpdateItem(input);

			expect(input).toEqual(original);
		});

		it('should handle already trimmed strings', () => {
			const result = getTrimedUserUpdateItem(mockUser);

			expect(result.firstName).toBe('John');
			expect(result.lastName).toBe('Doe');
		});
	});

	describe('getTrimedString', () => {
		it('should trim leading and trailing spaces', () => {
			expect(getTrimedString('  hello  ')).toBe('hello');
		});

		it('should replace multiple spaces with single space', () => {
			expect(getTrimedString('hello    world')).toBe('hello world');
		});

		it('should handle tabs and newlines', () => {
			expect(getTrimedString('hello\t\n  world')).toBe('hello world');
		});

		it('should handle mixed whitespace characters', () => {
			expect(getTrimedString('  hello\t\t\nworld  ')).toBe('hello world');
		});

		it('should return empty string for whitespace-only input', () => {
			expect(getTrimedString('   \t\n  ')).toBe('');
		});

		it('should handle already normalized strings', () => {
			expect(getTrimedString('hello world')).toBe('hello world');
		});

		it('should handle empty string', () => {
			expect(getTrimedString('')).toBe('');
		});
	});

	describe('isStringArray', () => {
		it('should return true for string array', () => {
			expect(isStringArray(['hello', 'world'])).toBe(true);
		});

		it('should return false for number array', () => {
			expect(isStringArray([1, 2, 3])).toBe(false);
		});

		it('should return false for mixed array starting with number', () => {
			expect(isStringArray([1, 'hello'])).toBe(false);
		});

		it('should return true for mixed array starting with string', () => {
			expect(isStringArray(['hello', 1])).toBe(true);
		});

		it('should return false for array with null first element', () => {
			expect(isStringArray([null, 'hello'])).toBe(false);
		});

		it('should return false for empty array', () => {
			expect(isStringArray([])).toBe(false);
		});
	});

	describe('setNumbersType1Value', () => {
		let mockSetFieldValue: jest.MockedFunction<SetFieldValue<unknown>>;

		beforeEach(() => {
			mockSetFieldValue = jest.fn();
		});

		it('should extract digits with default max chars', () => {
			const handler = setNumbersType1Value(mockSetFieldValue);
			handler('testField', 'abc123def456', false);

			expect(mockSetFieldValue).toHaveBeenCalledWith('testField', '', false);
		});

		it('should limit digits to maxChars', () => {
			const handler = setNumbersType1Value(mockSetFieldValue, 3);
			handler('testField', '123456789');

			expect(mockSetFieldValue).toHaveBeenCalledWith(
				'testField',
				'123',
				undefined
			);
		});

		it('should handle non-string values', () => {
			const handler = setNumbersType1Value(mockSetFieldValue, 5);
			handler('testField', 12345);

			expect(mockSetFieldValue).toHaveBeenCalledWith(
				'testField',
				'12345',
				undefined
			);
		});

		it('should return empty string for no digits', () => {
			const handler = setNumbersType1Value(mockSetFieldValue);
			handler('testField', 'abc');

			expect(mockSetFieldValue).toHaveBeenCalledWith(
				'testField',
				'',
				undefined
			);
		});

		it('should handle empty string', () => {
			const handler = setNumbersType1Value(mockSetFieldValue);
			handler('testField', '');

			expect(mockSetFieldValue).toHaveBeenCalledWith(
				'testField',
				'',
				undefined
			);
		});

		it('should handle null and undefined', () => {
			const handler = setNumbersType1Value(mockSetFieldValue);

			handler('testField', null);
			expect(mockSetFieldValue).toHaveBeenCalledWith(
				'testField',
				'',
				undefined
			);

			handler('testField', undefined);
			expect(mockSetFieldValue).toHaveBeenCalledWith(
				'testField',
				'',
				undefined
			);
		});

		it('should pass shouldValidate parameter', () => {
			const handler = setNumbersType1Value(mockSetFieldValue);
			handler('testField', '123', true);

			expect(mockSetFieldValue).toHaveBeenCalledWith('testField', '123', true);
		});
	});

	describe('mapNamesFromArray', () => {
		it('should extract names from array of objects', () => {
			const input = [
				{ name: 'John', id: 1 },
				{ name: 'Jane', id: 2 },
			];
			expect(mapNamesFromArray(input)).toEqual(['John', 'Jane']);
		});

		it('should return empty array for undefined input', () => {
			expect(mapNamesFromArray(undefined)).toEqual([]);
		});

		it('should return empty array for empty array', () => {
			expect(mapNamesFromArray([])).toEqual([]);
		});

		it('should preserve original array', () => {
			const input = [{ name: 'Test', id: 1 }];
			const original = [...input];
			mapNamesFromArray(input);
			expect(input).toEqual(original);
		});
	});

	describe('lowercaseFirstSymbol', () => {
		it('should lowercase first character', () => {
			expect(lowercaseFirstSymbol('Hello')).toBe('hello');
		});

		it('should trim spaces and lowercase first character', () => {
			expect(lowercaseFirstSymbol('  World  ')).toBe('world');
		});

		it('should handle single character', () => {
			expect(lowercaseFirstSymbol('A')).toBe('a');
		});

		it('should handle empty string after trim', () => {
			expect(lowercaseFirstSymbol('   ')).toBe('');
		});

		it('should handle already lowercase first character', () => {
			expect(lowercaseFirstSymbol('hello')).toBe('hello');
		});

		it('should handle non-alphabetic first character', () => {
			expect(lowercaseFirstSymbol('123abc')).toBe('123abc');
		});
	});

	describe('getIdFromPathname', () => {
		it('should return last segment from pathname', () => {
			expect(getIdFromPathname('/users/profile/123')).toBe('123');
		});

		it('should handle single segment', () => {
			expect(getIdFromPathname('homepage')).toBe('homepage');
		});

		it('should handle root path', () => {
			expect(getIdFromPathname('/')).toBe('');
		});

		it('should handle trailing slash', () => {
			expect(getIdFromPathname('/users/123/')).toBe('');
		});

		it('should handle empty string', () => {
			expect(getIdFromPathname('')).toBe('');
		});

		it('should handle multiple consecutive slashes', () => {
			expect(getIdFromPathname('/users//123')).toBe('123');
		});
	});

	describe('formatToFixedPrecision', () => {
		it('should return integer as is', () => {
			expect(formatToFixedPrecision(5)).toBe(5);
		});

		it('should format decimal with default precision', () => {
			expect(formatToFixedPrecision(5.123456)).toBe(5.12);
		});

		it('should format decimal with custom precision', () => {
			expect(formatToFixedPrecision(5.123456, 3)).toBe(5.123);
		});

		it('should handle zero', () => {
			expect(formatToFixedPrecision(0)).toBe(0);
		});

		it('should handle negative numbers', () => {
			expect(formatToFixedPrecision(-5.789, 1)).toBe(-5.8);
		});

		it('should round properly', () => {
			expect(formatToFixedPrecision(5.996, 2)).toBe(6);
		});
	});

	describe('formatDiscountPercent', () => {
		it('should format range when both values provided', () => {
			expect(formatDiscountPercent(10, 20)).toBe('10-20%');
		});

		it('should format single value when only min provided', () => {
			expect(formatDiscountPercent(15, null)).toBe('15%');
		});

		it('should format single value when only max provided', () => {
			expect(formatDiscountPercent(null, 25)).toBe('25%');
		});

		it('should return empty string when both are null', () => {
			expect(formatDiscountPercent(null, null)).toBe('');
		});

		it('should return empty string when both are undefined', () => {
			expect(formatDiscountPercent()).toBe('');
		});
	});

	describe('checkIsTextCut', () => {
		let mockElement: Partial<HTMLElement>;

		beforeEach(() => {
			mockElement = {
				offsetWidth: 100,
				scrollWidth: 100,
			};
		});

		it('should return false when text is not cut', () => {
			expect(checkIsTextCut(mockElement as HTMLElement)).toBe(false);
		});

		it('should return true when text is cut', () => {
			Object.defineProperty(mockElement, 'scrollWidth', { value: 150 });
			expect(checkIsTextCut(mockElement as HTMLElement)).toBe(true);
		});

		it('should handle scrollOffsetSubstraction', () => {
			Object.defineProperty(mockElement, 'scrollWidth', { value: 110 });
			expect(checkIsTextCut(mockElement as HTMLElement, 15)).toBe(false);
		});

		it('should handle undefined scrollOffsetSubstraction', () => {
			Object.defineProperty(mockElement, 'scrollWidth', { value: 120 });
			expect(checkIsTextCut(mockElement as HTMLElement, undefined)).toBe(true);
		});

		it('should handle zero scrollOffsetSubstraction', () => {
			Object.defineProperty(mockElement, 'scrollWidth', { value: 120 });
			expect(checkIsTextCut(mockElement as HTMLElement, 0)).toBe(true);
		});
	});

	describe('getDialogTextComponent', () => {
		const mockParams = {
			isBlock: false,
			userId: 1,
			firstName: 'John',
			lastName: 'Doe',
			isForGuest: false,
			guestName: null,
		};

		beforeEach(() => {
			(TextWithStrongInside as jest.Mock).mockReturnValue(<div>mocked</div>);
		});

		it('should return null when isBlock true and not for guest', () => {
			const result = getDialogTextComponent(
				{ ...mockParams, isBlock: true },
				2,
				mockBookingT
			);
			expect(result).toBeNull();
		});

		it('should return null for same user', () => {
			const result = getDialogTextComponent(mockParams, 1, mockBookingT);
			expect(result).toBeNull();
		});

		it('should return component for guest even with same userId', () => {
			const guestParams = {
				...mockParams,
				isForGuest: true,
				guestName: 'Guest',
			};
			const result = getDialogTextComponent(guestParams, 1, mockBookingT);
			expect(result).not.toBeNull();
		});
	});
});
