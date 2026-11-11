import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import {
	DEFAULT_DISABLED_INPUT_COMPONENT_OPACITY,
	DEFAULT_OPACITY,
} from '@/constants/opacities';

export const DoubleColumnContainer = styled('div')(
	({ theme }) => css`
		display: grid;
		grid-template-columns: repeat(2, minmax(0, 1fr));
		column-gap: ${theme.spaces.s}px;
	`
);

export const SwitchContainer = styled('div')(
	({ theme }) => css`
		display: flex;
		justify-content: space-between;
		gap: ${theme.spaces.s}px;
	`
);

export const SwitchLabel = styled(Typography)<{ disabled?: boolean }>(
	({ disabled, theme }) => ({
		color: theme.palette['text-gray'],
		opacity: disabled
			? DEFAULT_DISABLED_INPUT_COMPONENT_OPACITY
			: DEFAULT_OPACITY,
	})
);
