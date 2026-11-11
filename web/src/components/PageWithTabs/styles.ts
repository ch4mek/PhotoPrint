import styled from '@emotion/styled';
import { Scrollbar } from '@mms/mms-ui-library/components/Scrollbar';

export const PageWithTabsWrapper = styled('div')({
	display: 'flex',
	flexDirection: 'column',
	height: '100%',
});

export const PageWithTabsContentWrapper = styled(Scrollbar)<{
	withPaddings: boolean;
}>(({ theme, withPaddings }) => ({
	flexGrow: 1,
	overflow: 'auto',

	...(withPaddings && {
		padding: `0px ${theme.spaces.m}px ${theme.spaces.m}px ${theme.spaces.m}px`,
	}),
}));
