import styled from '@emotion/styled';

import { SalaryCellWrapperProps } from './types';

export const SalaryCellWrapper = styled('div')<SalaryCellWrapperProps>(
	({ theme, isCurrent }) => ({
		color: isCurrent
			? theme.palette['text-accent']
			: theme.palette['text-black'],
	})
);
