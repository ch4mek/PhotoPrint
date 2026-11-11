import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { popUpBreakpoints } from '@/constants/popUp';

export const ThreeInputsRowWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: `repeat(3, 1fr)`,
	gap: theme.spaces.s,
	alignItems: 'center',

	[theme.breakpoints.down(popUpBreakpoints.tabletXL)]: {
		gridTemplateColumns: 'repeat(2, 1fr)',
		rowGap: theme.spaces.l,

		'& > :nth-child(3)': {
			gridColumn: '1 / -1',
		},
	},
}));

export const KitAmountSection = styled('div')(({ theme }) => ({
	display: 'flex',
	width: '100%',
	alignItems: 'flex-start',
	gap: theme.spaces.s,

	'& > div': {
		minHeight: 40,
		minWidth: 110,
	},
}));

export const KitName = styled('div')(({ theme }) => ({
	flex: 1,
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,
}));

export const AvailableKits = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-gray'],
}));
