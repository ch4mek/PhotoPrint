import styled from '@emotion/styled';

export const MeetingRoomsRendererWrapper = styled('div')(({ theme }) => ({
	flexGrow: 1,
	minHeight: 0,
	padding: `0 ${theme.spaces.m}px`,
}));
