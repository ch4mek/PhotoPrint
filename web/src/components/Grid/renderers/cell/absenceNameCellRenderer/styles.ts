import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { IconSize } from '@/constants/iconSizes';

import { IconContainerProps } from './types';

export const AbsenceCellWrapper = styled('div')(({ theme }) => ({
	height: '100%',
	width: '100%',
	padding: `${theme.spaces.xs}px 0`,
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
}));

export const IconContainer = styled('div')<IconContainerProps>(
	({ background, color, theme }) => ({
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'center',
		background,
		color,
		borderRadius: BorderRadiuses.xs2,
		padding: theme.spaces.xs,
		width: IconSize.M,
		height: IconSize.M,

		'& svg': {
			width: IconSize.S,
			height: IconSize.S,
		},
	})
);

export const TextOverflow = styled(Typography)({
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	whiteSpace: 'nowrap',
});
