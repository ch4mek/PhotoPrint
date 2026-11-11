import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { BOX_RANDOM_COLOR_SIZE } from '@/constants/commonStyles';
import { DefaultBoxRandomColorsProps } from '@/types/boxRandomColors';

export const BoxRandomColorItem = styled('div')(
	({ color, disabled }: DefaultBoxRandomColorsProps) => ({
		background: color,
		borderRadius: BorderRadiuses.xs,
		width: BOX_RANDOM_COLOR_SIZE,
		height: BOX_RANDOM_COLOR_SIZE,
		cursor: disabled ? 'auto' : 'pointer',
		gridArea: 'colors',
	})
);
