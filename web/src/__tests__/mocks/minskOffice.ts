import { Timezone } from '@/components/Timeline/types';
import { MinskSpacesInternalCodes } from '@/types/booking';
import { LocationTypeId, OfficeType } from '@/types/configuration';

export const officeMock: OfficeType = {
	id: 1,
	name: 'Minsk',
	timeZone: Timezone.MINSK,
	timeZoneDisplayName: 'Minsk',
	location: LocationTypeId.Minsk,
	workspaces: [
		{
			id: 1,
			name: 'Space 403',
			internalCode: MinskSpacesInternalCodes.SpaceA,
		},
		{
			id: 2,
			name: 'Space 404',
			internalCode: MinskSpacesInternalCodes.SpaceB,
		},
		{
			id: 3,
			name: 'Space 405',
			internalCode: MinskSpacesInternalCodes.SpaceC,
		},
		{
			id: 4,
			name: 'Space 406',
			internalCode: MinskSpacesInternalCodes.SpaceD,
		},
	],
	meetingRooms: [
		{
			id: 3,
			name: 'Meeting room 403.2',
		},
		{
			id: 2,
			name: 'Meeting room 403.1',
		},
		{
			id: 4,
			name: 'Meeting room 407',
		},
	],
};
