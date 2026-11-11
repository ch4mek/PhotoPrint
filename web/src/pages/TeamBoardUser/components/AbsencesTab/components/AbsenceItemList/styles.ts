import styled from '@emotion/styled';

import { MIN_ABSENCE_CARDS_HEIGHT } from '@/constants/absence';

export const AbsenceListWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	overflow: 'hidden',
	maxHeight: MIN_ABSENCE_CARDS_HEIGHT,
	marginBottom: theme.spaces.m,
}));
