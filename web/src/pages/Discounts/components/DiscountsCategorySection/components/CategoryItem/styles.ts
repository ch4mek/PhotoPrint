import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const ITEM_LINE_HEIGHT = 20;

export const CategoryItemWrapper = styled('div')<{ isSelected: boolean }>(
	({ theme, isSelected }) => ({
		display: 'flex',

		alignItems: 'start',
		cursor: 'pointer',
		WebkitUserSelect: 'none',
		msUserSelect: 'none',
		userSelect: 'none',
		color: theme.palette['text-black'],
		...theme.typography.body['m-400'],
		whiteSpace: 'normal',
		padding: theme.spaces.s,
		lineHeight: `${ITEM_LINE_HEIGHT}px`,

		border: `1px solid ${theme.palette['graphics-divider-1']}`,
		borderRadius: BorderRadiuses.m,

		...(isSelected && {
			backgroundColor: theme.palette['background-primary-1'],
		}),
	})
);

export const CategoryItemContent = styled('div')({
	flex: 1,
	alignSelf: 'stretch',

	display: 'flex',
	alignItems: 'center',

	wordBreak: 'break-word',
	overflowWrap: 'break-word',
	whiteSpace: 'normal',
});
