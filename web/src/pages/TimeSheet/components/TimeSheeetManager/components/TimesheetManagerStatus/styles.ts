import styled from '@emotion/styled';

import { SMALL_ICON_SIZE } from '@/constants/commonStyles';
import { TIMESHEET_STATUS } from '@/constants/gridAreaNames';

export const TimesheetStatusWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	flexDirection: 'row-reverse',
	justifyContent: 'space-between',
	gridArea: TIMESHEET_STATUS,
	gap: theme.spaces.s,

	'& > button': {
		height: SMALL_ICON_SIZE,
		width: SMALL_ICON_SIZE,
	},
}));
