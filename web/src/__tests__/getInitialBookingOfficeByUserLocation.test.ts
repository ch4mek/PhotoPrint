import { Timezone } from '@/components/Timeline/types';
import { getInitialBookingOfficeByUserLocation } from '@/helpers/getInitialBookingOfficeByUserLocation';
import { LocationTypeId, OfficeType } from '@/types/configuration';

describe('Helper function getInitialBookingOfficeByUserLocation', () => {
	const offices: OfficeType[] = [
		{
			id: 1,
			name: 'Minsk Office',
			location: LocationTypeId.Minsk,
			timeZone: Timezone.MINSK,
			timeZoneDisplayName: '',
			workspaces: [],
			meetingRooms: [],
		},
		{
			id: 2,
			name: 'Warsaw Office',
			location: LocationTypeId.Warsaw,
			timeZone: Timezone.WARSAW,
			timeZoneDisplayName: '',
			workspaces: [],
			meetingRooms: [],
		},
		{
			id: 3,
			name: 'Tbilisi Office',
			location: LocationTypeId.Tbilisi,
			timeZone: Timezone.WARSAW,
			timeZoneDisplayName: '',
			workspaces: [],
			meetingRooms: [],
		},
	];

	it('should return exact match for user location', () => {
		const office = getInitialBookingOfficeByUserLocation(
			offices,
			LocationTypeId.Warsaw
		);
		expect(office).toEqual(offices[1]);
	});

	it('should fallback to Minsk for Vitebsk location', () => {
		const office = getInitialBookingOfficeByUserLocation(
			offices,
			LocationTypeId.Vitebsk
		);
		expect(office).toEqual(offices[0]);
	});

	it('should fallback to Warsaw for Tbilisi location', () => {
		const office = getInitialBookingOfficeByUserLocation(
			offices,
			LocationTypeId.Tbilisi
		);
		expect(office).toEqual(offices[2]);
	});

	it('should return null if no matching office and no fallback', () => {
		const office = getInitialBookingOfficeByUserLocation(
			offices,
			undefined as unknown as LocationTypeId
		);
		expect(office).toBeNull();
	});

	it('should return nearest Minsk office if location is Minsk and office exists', () => {
		const office = getInitialBookingOfficeByUserLocation(
			offices,
			LocationTypeId.Minsk
		);
		expect(office).toEqual(offices[0]);
	});

	it('should return null if offices array is empty', () => {
		const office = getInitialBookingOfficeByUserLocation(
			[],
			LocationTypeId.Minsk
		);
		expect(office).toBeNull();
	});
});
