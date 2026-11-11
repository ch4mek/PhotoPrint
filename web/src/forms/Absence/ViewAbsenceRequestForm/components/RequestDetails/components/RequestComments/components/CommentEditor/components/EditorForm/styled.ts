import { Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';
import { CSSProperties } from 'react';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';

import { MIN_PLATE_HEIGHT } from './constants';

export const CommentEditorWrapper = styled('div')<{ hasFiles?: boolean }>(
	({ theme, hasFiles = false }) => ({
		display: 'flex',
		flexDirection: 'column',

		border: `1px solid ${theme.palette['primary-4']}`,
		borderRadius: BorderRadiuses.m,

		overflow: 'hidden',
		transition: 'all 0.3s ease-in-out',

		...(hasFiles && {
			borderTopLeftRadius: 0,
			borderTopRightRadius: 0,
		}),
	})
);

export const PlateWrapper = styled('div')(({ theme }) => ({
	flexGrow: 1,
	overflowY: 'auto',

	backgroundColor: 'transparent',
	borderRadius: BorderRadiuses.m,

	height: MIN_PLATE_HEIGHT,

	whiteSpace: 'normal',
	wordBreak: 'break-word',

	padding: `${theme.spaces.s}px ${theme.spaces.m}px`,

	...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),
}));

export const FormControlsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	marginTop: `${theme.spaces.s}px`,
}));

export const getPlaceholderStyles = ({
	theme,
	isEmpty = true,
}: {
	theme: Theme;
	isEmpty: boolean;
}): CSSProperties => ({
	color: !isEmpty
		? `${theme.palette['text-black']} !important`
		: `${theme.palette['text-gray']} !important`,
});
