import { TeamBoardTabsId } from '../../../types';

import { SortDirection } from '@/constants/sort';

export const sectionQueryParams = {
	skip: 0,
	top: 1,
	orderBy: `date ${SortDirection.DESC}`,
};

export const CONVERSATIONS_DATA_CONTAINER_PROPS = {
	headerTitle: 'Conversations',
	tabId: TeamBoardTabsId.CONVERSATIONS,
};
