import { filterUniqueWorkplaces } from '@/helpers/filterUniqueWorkplaces';
import { Workplace } from '@/types/booking';

describe('Helper function filterUniqueWorkplaces', () => {
	const workplaces: Workplace[] = [
		{
			bookingId: 1,
			workplaceNumber: 101,
			workplaceId: 1001,
			userId: 1,
			firstName: 'Alice',
			lastName: 'A',
			department: 'Dept1',
			isBlocked: false,
			isDisabled: false,
			isForGuest: false,
			guestName: null,
		},
		{
			bookingId: 2,
			workplaceNumber: 102,
			workplaceId: 1002,
			userId: 2,
			firstName: 'Bob',
			lastName: 'B',
			department: 'Dept1',
			isBlocked: false,
			isDisabled: false,
			isForGuest: false,
			guestName: null,
		},
		{
			bookingId: 3,
			workplaceNumber: 101,
			workplaceId: 1003,
			userId: 3,
			firstName: 'Charlie',
			lastName: 'C',
			department: 'Dept2',
			isBlocked: false,
			isDisabled: false,
			isForGuest: false,
			guestName: null,
		},
	];

	it('Should filter out workplaces with duplicate workplaceNumber', () => {
		const result = filterUniqueWorkplaces(workplaces);
		expect(result).toEqual([workplaces[0], workplaces[1]]);
	});

	it('Should return the same array if all workplaceNumbers are unique', () => {
		const uniqueWorkplaces: Workplace[] = [
			{
				bookingId: 4,
				workplaceNumber: 201,
				workplaceId: 2001,
				userId: 4,
				firstName: 'X',
				lastName: 'X',
				department: 'DeptX',
				isBlocked: false,
				isDisabled: false,
				isForGuest: false,
				guestName: null,
			},
			{
				bookingId: 5,
				workplaceNumber: 202,
				workplaceId: 2002,
				userId: 5,
				firstName: 'Y',
				lastName: 'Y',
				department: 'DeptY',
				isBlocked: false,
				isDisabled: false,
				isForGuest: false,
				guestName: null,
			},
		];
		const result = filterUniqueWorkplaces(uniqueWorkplaces);
		expect(result).toEqual(uniqueWorkplaces);
	});

	it('Should handle empty array and return empty array', () => {
		const result = filterUniqueWorkplaces([]);
		expect(result).toEqual([]);
	});

	it('Should keep the first occurrence of a duplicate workplaceNumber', () => {
		const duplicates: Workplace[] = [
			{
				bookingId: 6,
				workplaceNumber: 1,
				workplaceId: 3001,
				userId: 6,
				firstName: 'First',
				lastName: 'F',
				department: 'D1',
				isBlocked: false,
				isDisabled: false,
				isForGuest: false,
				guestName: null,
			},
			{
				bookingId: 7,
				workplaceNumber: 1,
				workplaceId: 3002,
				userId: 7,
				firstName: 'Second',
				lastName: 'S',
				department: 'D1',
				isBlocked: false,
				isDisabled: false,
				isForGuest: false,
				guestName: null,
			},
			{
				bookingId: 8,
				workplaceNumber: 2,
				workplaceId: 3003,
				userId: 8,
				firstName: 'Third',
				lastName: 'T',
				department: 'D2',
				isBlocked: false,
				isDisabled: false,
				isForGuest: false,
				guestName: null,
			},
		];
		const result = filterUniqueWorkplaces(duplicates);
		expect(result).toEqual([duplicates[0], duplicates[2]]);
	});
});
