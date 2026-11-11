import styled from '@emotion/styled';

import { verticalEllipsisStyles } from '@/components/Grid/renderers/styles';
import { CellWrapperProps } from '@/pages/Payload/types';

export const CellWrapper = styled('div')<CellWrapperProps>(
	({
		theme,
		childHeight = '100%',
		childWidth,
		maxVerticalLineCount,
		isExpanded,
	}) => ({
		padding: `${theme.spaces.m - 1}px ${theme.spaces.s}px`,
		display: 'flex',
		width: '100%',
		height: '100%',

		...theme.typography.body['m-400'],

		'& > *': {
			...(maxVerticalLineCount && verticalEllipsisStyles(maxVerticalLineCount)),
			WebkitLineClamp: !isExpanded ? 1 : 'auto',
			width: childWidth,
			height: `${childHeight} !important`,
		},
	})
);
