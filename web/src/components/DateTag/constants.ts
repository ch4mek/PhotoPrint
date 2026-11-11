import {
	GREEN_TAG_COLOR,
	ORANGE_TAG_COLOR,
	RED_TAG_COLOR,
	WHITE,
	YELLOW_TAG_COLOR,
} from '../../theme';

import { TagColors, TagTypes } from '@/components/DateTag/types';

export const tagColors: TagColors = {
	[TagTypes.Future]: {
		backgroundColor: WHITE,
		nameColor: 'future',
		descriptionColor: 'future',
	},
	[TagTypes.Upcoming]: {
		backgroundColor: YELLOW_TAG_COLOR,
		nameColor: 'upcoming',
		descriptionColor: 'upcoming',
	},
	[TagTypes.Imminent]: {
		backgroundColor: ORANGE_TAG_COLOR,
		nameColor: 'imminent',
		descriptionColor: 'imminent',
	},
	[TagTypes.OverdueMoreThanNDays]: {
		backgroundColor: RED_TAG_COLOR,
		nameColor: 'overdueMoreThanNDays',
		descriptionColor: 'overdueMoreThanNDays',
	},
	[TagTypes.Passed]: {
		backgroundColor: GREEN_TAG_COLOR,
		nameColor: 'passed',
		descriptionColor: 'passed',
	},
};
