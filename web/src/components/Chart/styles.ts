import { css } from '@emotion/react';
import styled from '@emotion/styled';

const OUTER_BUG_FIX_BOX_PADDING_BOTTOM = 224;

export const OuterBugFixBox = styled('div')({
	position: 'relative',
	width: '100%',
	paddingBottom: OUTER_BUG_FIX_BOX_PADDING_BOTTOM,
});

export const InnerBugFixBox = styled('div')(
	() => css`
		position: absolute;
		left: 0;
		right: 0;
		bottom: 0;
		top: 0;
	`
);

export const Text = styled('text')(({ theme }) => ({
	color: theme.palette['text-gray'],
	fill: theme.palette['text-gray'],
	...theme.typography.body['s-400'],
}));
