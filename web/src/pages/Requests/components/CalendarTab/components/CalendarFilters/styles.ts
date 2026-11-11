import styled from '@emotion/styled';

import { AUTOSELECT_FILTER_WIDTH } from '@/constants/commonStyles';

export const AutoselectWrapper = styled('div')(({ theme }) => ({
	width: AUTOSELECT_FILTER_WIDTH,
	background: theme.palette['background-white'],
}));

export const CalendarFiltersWrapper = styled('div')(({ theme }) => ({
	paddingBlock: theme.spaces.s,
	display: 'flex',
	flexDirection: 'row',
	flexWrap: 'wrap',
	gap: theme.spaces.s,
}));

export const ButtonWithFilterWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
}));
