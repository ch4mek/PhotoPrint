import {
	areArraysEqualByPrimitiveItems,
	areDateRangesEqual,
	areFiltersEqualByKeys,
	areFromToCompatibleObjectsEqual,
} from '../helpers/areValuesEqual';

import { DEFAULT_DATE_RANGE_VALUE } from '@/components/CommonProjectAccessTab/constants';
import { DateRangeFilter, FromToProps } from '@/components/Grid/modals/types';

const today = new Date();

const addDaysToDate = (date: Date, days: number) => {
	const newDate = new Date(date);
	newDate.setDate(date.getDate() + days);

	return newDate;
};

const dateRangesTestCases: Array<[DateRangeFilter, DateRangeFilter, boolean]> =
	[
		[DEFAULT_DATE_RANGE_VALUE, DEFAULT_DATE_RANGE_VALUE, true],
		[
			{
				...DEFAULT_DATE_RANGE_VALUE,
				includeNull: true,
			},
			DEFAULT_DATE_RANGE_VALUE,
			false,
		],
		[
			{
				...DEFAULT_DATE_RANGE_VALUE,
				toDate: new Date(),
			},
			DEFAULT_DATE_RANGE_VALUE,
			false,
		],
		[
			DEFAULT_DATE_RANGE_VALUE,
			{
				...DEFAULT_DATE_RANGE_VALUE,
				fromDate: new Date(),
			},
			false,
		],
		[
			{
				...DEFAULT_DATE_RANGE_VALUE,
				fromDate: today,
			},
			{
				...DEFAULT_DATE_RANGE_VALUE,
				fromDate: new Date(today),
			},
			true,
		],
		[
			{
				...DEFAULT_DATE_RANGE_VALUE,
				toDate: today,
			},
			{
				...DEFAULT_DATE_RANGE_VALUE,
				toDate: addDaysToDate(today, 1),
			},
			false,
		],
	];

describe('areDateRangeEqual helper', () => {
	it.each(dateRangesTestCases)(
		'%s and %s should be %s',
		(date1, date2, expectedResult) => {
			expect(areDateRangesEqual(date1, date2)).toBe(expectedResult);
		}
	);
});

const arrayWithPrimitiveValuesTestCases: Array<
	[Array<string | number>, Array<string | number>, boolean]
> = [
	[[], [], true],
	[[1, 2, 3], [1, 2, 3], true],
	[['1', '2', '3'], ['1', '2', '3'], true],
	[['1', '2'], ['1', '2', '3'], false],
	[[1, 2], [3, 4], false],
	[['1', '2'], ['3', '4'], false],
];

describe('areArraysEqualByPrimitiveItems', () => {
	it.each(arrayWithPrimitiveValuesTestCases)(
		'%s and %s should be %s',
		(array1, array2, expectedResult) => {
			expect(areArraysEqualByPrimitiveItems(array1, array2)).toBe(
				expectedResult
			);
		}
	);
});

const fromToTestCases: Array<
	[FromToProps<string | Date>, FromToProps<string | Date>, boolean]
> = [
	[{ from: null, to: null }, { from: null, to: null }, true],
	[{ from: '0', to: '1' }, { from: '0', to: '1' }, true],
	[{ from: '1', to: '1' }, { from: '0', to: '1' }, false],
	[{ from: '1', to: '1' }, { from: '1', to: '0' }, false],
	[{ from: today, to: null }, { from: new Date(today), to: null }, true],
	[
		{ from: today, to: null },
		{ from: addDaysToDate(today, 1), to: null },
		false,
	],
];

describe('areFromToCompatibleObjectsEqual', () => {
	it.each(fromToTestCases)(
		'%s and %s should be %s',
		(fromTo1, fromTo2, expectedResult) => {
			expect(areFromToCompatibleObjectsEqual(fromTo1, fromTo2)).toBe(
				expectedResult
			);
		}
	);
});

describe('areFiltersEqualByKeys', () => {
	it('returns true only when every listed key matches', () => {
		const current = { a: '1', b: ['x'], c: { from: null, to: null } };
		const initial = { ...current };

		expect(
			areFiltersEqualByKeys(current, initial, ['a', 'b', 'c'] as const)
		).toBe(true);

		expect(
			areFiltersEqualByKeys({ ...current, a: '2' }, initial, [
				'a',
				'b',
				'c',
			] as const)
		).toBe(false);
	});

	it('compares field types like dedicated helpers (arrays, primitives, from/to)', () => {
		expect(
			areFiltersEqualByKeys({ k: ['a', 'b'] }, { k: ['b', 'a'] }, ['k'])
		).toBe(true);
		expect(areFiltersEqualByKeys({ k: 'x' }, { k: 'x' }, ['k'])).toBe(true);
		expect(
			areFiltersEqualByKeys(
				{ k: { from: today, to: null } },
				{ k: { from: new Date(today), to: null } },
				['k']
			)
		).toBe(true);
	});

	it('returns false when value shapes differ for a key', () => {
		expect(
			areFiltersEqualByKeys({ k: [] }, { k: { from: null, to: null } }, ['k'])
		).toBe(false);
		expect(areFiltersEqualByKeys({ k: 'a' }, { k: ['a'] }, ['k'])).toBe(false);
		expect(
			areFiltersEqualByKeys({ k: { from: '1', to: null } }, { k: null }, ['k'])
		).toBe(false);
	});
});
