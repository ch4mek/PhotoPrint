import styled from '@emotion/styled';

import { BOOKING_MOBILE_SCREEN_WIDTH } from '../../../../constants';

import { switchGridAreas } from './constants';
import { ButtonsWrapperProps } from './types';

const { repetativeAccordion, repetativeSwitch, allDaySwitch } = switchGridAreas;

const SWITCH_CONTAINER_TRANSITION = 'all 0.3s ease-in-out';
const CONTROLS_FIRST_COLUMN_WIDTH = 157;

export const Wrapper = styled('div')(
	({ theme }) =>
		({
			flexGrow: 1,
			minHeight: 'fit-content',
			display: 'flex',
			flexDirection: 'column',
			justifyContent: 'space-between',
			width: '100%',
			paddingBottom: theme.spaces.m,
		} as const)
);

export const ButtonsWrapper = styled('div')<ButtonsWrapperProps>(
	({ theme, isChiefRole }) => ({
		display: 'flex',
		justifyContent: isChiefRole ? 'space-between' : 'unset',
		gap: theme.spaces.s,

		button: {
			flexShrink: 1,
		},

		[theme.breakpoints.down(BOOKING_MOBILE_SCREEN_WIDTH)]: {
			paddingBottom: 0,
		},
	})
);

export const SwitchContainer = styled('div')<{ isTwoRows: boolean }>(
	({ theme, isTwoRows }) => ({
		display: 'grid',
		gridTemplateAreas: `"${repetativeSwitch} ${allDaySwitch}" "${repetativeAccordion} ${repetativeAccordion}"`,
		gridTemplateColumns: `${CONTROLS_FIRST_COLUMN_WIDTH}px 1fr`,
		columnGap: theme.spaces.l,
		rowGap: isTwoRows ? theme.spaces.m : 0,
		marginBottom: isTwoRows ? 0 : theme.spaces.m,
		transition: SWITCH_CONTAINER_TRANSITION,
	})
);
