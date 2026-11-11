import styled from '@emotion/styled';
import { Scrollbar } from '@mms/mms-ui-library/components/Scrollbar';
import { spaces } from '@mms/mms-ui-library/theme';

import {
	DEFAULT_OPACITY,
	TIMELINE_DISABLED_OPACITY,
} from '@/constants/opacities';

export const Row = styled('div')<{ spacing?: keyof typeof spaces }>(
	({ theme, spacing }) => ({
		display: 'flex',
		gap: spacing ? theme.spaces[spacing] : 0,
		alignItems: 'center',
	})
);

export const TimelineWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
	maxHeight: '100%',
	containerType: 'inline-size',
}));

export const TimelineTableWrapper = styled(Scrollbar)(() => ({
	height: '100%',
	flexGrow: 1,
	minHeight: 0,
}));

export const TimelineTable = styled('div')<{
	isPeriodSelectionDisabled: boolean;
}>(({ isPeriodSelectionDisabled }) => ({
	display: 'flex',
	opacity: isPeriodSelectionDisabled
		? TIMELINE_DISABLED_OPACITY
		: DEFAULT_OPACITY,
}));

export const MobileSwitchWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	width: '100%',
	justifyContent: 'space-between',
	gap: theme.spaces.s,
}));
