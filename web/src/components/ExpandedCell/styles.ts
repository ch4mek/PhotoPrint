import styled from '@emotion/styled';

import { SMALL_ICON_SIZE } from '@/constants/commonStyles';
import { TIMESHEET_EXPANDER } from '@/constants/gridAreaNames';

export const CellExpanderFormatterWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifySelf: 'flex-end',
	gridArea: TIMESHEET_EXPANDER,
	paddingLeft: theme.spaces.xs,

	'& > button': {
		height: SMALL_ICON_SIZE,
		width: SMALL_ICON_SIZE,
	},
}));
