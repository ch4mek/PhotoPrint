import { HIDDEN_MEETING_ROOMS, HIDDEN_WORKSPACES } from '@/constants/booking';
import { OfficeType } from '@/types/configuration';

import { filterHiddenItemsByName } from './filterHiddenItems';
import { sortByName } from './sortByKey';

// TODO: remove within MMS2-6978
const filterOfficeItems = (office: OfficeType) => ({
	...office,
	workspaces: filterHiddenItemsByName(office.workspaces, HIDDEN_WORKSPACES),
	meetingRooms: filterHiddenItemsByName(
		office.meetingRooms,
		HIDDEN_MEETING_ROOMS
	),
});

export const sortOfficeItems = (office: OfficeType) => {
	const sortedOfficeItems = {
		...office,
		workspaces: sortByName(office.workspaces),
		meetingRooms: sortByName(office.meetingRooms),
	};

	return filterOfficeItems(sortedOfficeItems);
};
