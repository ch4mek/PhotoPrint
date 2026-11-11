import styled from '@emotion/styled';
import { DATA_GRID_WRAPPER_CLASS_NAME } from '@mms/mms-ui-library/constants/grid';

const PAGINATION_HEIGHT = 52;
const CALENDAR_HEIGHT = 173;
const CONTROLER_HEIGHT = 36;

export const InfoDataContainerWrapper = styled('div')(({ theme }) => ({
	flexGrow: 1,
	height: `calc(100% - ${PAGINATION_HEIGHT}px - ${
		theme.spaces.s * 2
	}px - ${CALENDAR_HEIGHT}px - ${CONTROLER_HEIGHT}px)`,

	[`& > div`]: {
		height: '100%',
	},
}));

export const CalendarGridsContainerWrapper = styled('div')(() => ({
	flexGrow: 1,
	height: '90%',

	[`& .${DATA_GRID_WRAPPER_CLASS_NAME}`]: {
		borderRadius: 0,

		'&::before, &::after': {
			borderRadius: 0,
		},
	},
}));
