import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { CellTextProps } from '@/components/Grid/renderers/types';
import { IconSize } from '@/constants/iconSizes';
import {
	DEFAULT_DISABLED_GRID_CELL_OPACITY,
	DEFAULT_OPACITY,
} from '@/constants/opacities';

export const CellTextWrapper = styled(Typography)(({ theme }) => ({
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	whiteSpace: 'nowrap',
	maxWidth: `calc(100% - ${theme.spaces.s}px - ${IconSize.MS}px)`,
}));

export const CellWithIconWrapper = styled('div')<CellTextProps>(
	({ disabled, theme }) => ({
		display: 'flex',
		justifyContent: 'space-between',
		alignItems: 'center',
		gap: theme.spaces.s,
		height: '100%',
		width: '100%',

		opacity: disabled ? DEFAULT_DISABLED_GRID_CELL_OPACITY : DEFAULT_OPACITY,

		'& > button svg path': {
			vectorEffect: 'non-scaling-stroke',
		},
	})
);
