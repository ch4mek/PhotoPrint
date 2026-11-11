import { css } from '@emotion/react';
import styled from '@emotion/styled';

import {
	DEFAULT_DISABLED_INPUT_COMPONENT_OPACITY,
	DEFAULT_OPACITY,
} from '@/constants/opacities';

export const CellWrapper = styled.div<{ isActive: boolean }>(
	({ theme, isActive }) => css`
		height: 100%;
		width: 100%;
		display: flex;
		justify-content: space-between;
		align-items: center;
		padding-left: ${theme.spaces.m}px;
		opacity: ${!isActive
			? DEFAULT_DISABLED_INPUT_COMPONENT_OPACITY
			: DEFAULT_OPACITY};

		span {
			white-space: nowrap;
			overflow: hidden;
			text-overflow: ellipsis;
		}
	`
);
