import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const LEGEND_CIRCLE_SIZE = 10;

export const KPIWidgetWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
	width: '100%',
	height: '100%',
	paddingBottom: theme.spaces.s,
}));

export const LegendWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.m,
	alignSelf: 'end',
}));

export const LegendItem = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.xs,
}));

export const LegendItemCircle = styled('div')(
	({ color }: { color: string }) => ({
		width: LEGEND_CIRCLE_SIZE,
		height: LEGEND_CIRCLE_SIZE,
		borderRadius: BorderRadiuses.half,
		background: color,
	})
);

export const LegendItemText = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-gray'],
}));
