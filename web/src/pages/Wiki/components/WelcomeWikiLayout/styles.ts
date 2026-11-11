import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { WHITE } from '@mms/mms-ui-library/theme';

import layoutBackground from '@/assets/common/wikiLayoutBackground.jpg';

const WELCOME_WIKI_FONT_SIZE_TITLE = 103;
const WELCOME_WIKI_FONT_LINE_HEIGHT_TITLE = 107;
const WELCOME_WIKI_COLOR_ACCENT = '#FFAB36';

const WELCOME_WIKI_FONT_SIZE_SUBTITLE = 39;
const WELCOME_WIKI_TEXT_SHADOW = '2px 2px 8px #00000040';
const WELCOME_WIKI_COLOR_GRAY = '#E8E8E8';

export const WelcomeWikiLayoutWrapper = styled('div')(({ theme }) => ({
	position: 'relative',
	height: '100%',
	width: '100%',
	margin: 'auto',
	backgroundImage: `url(${layoutBackground})`,

	backgroundSize: 'auto 100%',
	backgroundRepeat: 'no-repeat',
	backgroundPosition: 'center center',

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		display: 'none',
	},
}));

export const HeadlineTitle = styled(Typography)(
	() => css`
		color: ${WHITE};
		font-size: ${WELCOME_WIKI_FONT_SIZE_TITLE}px;
		line-height: ${WELCOME_WIKI_FONT_LINE_HEIGHT_TITLE}px;
		text-shadow: ${WELCOME_WIKI_TEXT_SHADOW};
	`
);

export const HeadlineAccentTitle = styled(HeadlineTitle)(
	() => css`
		color: ${WELCOME_WIKI_COLOR_ACCENT};
	`
);

export const SubHeadlineTitle = styled(Typography)(
	() => css`
		color: ${WELCOME_WIKI_COLOR_GRAY};
		font-size: ${WELCOME_WIKI_FONT_SIZE_SUBTITLE}px;
		text-shadow: ${WELCOME_WIKI_TEXT_SHADOW};
	`
);

export const HeadlineWrapper = styled('div')(({ theme }) => ({
	height: '100%',
	width: '70%',
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'center',
	justifyContent: 'center',
	gap: `${theme.spaces.xl}px`,

	[theme.breakpoints.down(theme.breakpoints.desktopS)]: {
		width: '100%',
	},
}));
