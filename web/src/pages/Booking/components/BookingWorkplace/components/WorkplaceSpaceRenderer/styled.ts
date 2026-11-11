import styled from '@emotion/styled';
import { Scrollbar } from '@mms/mms-ui-library/components/Scrollbar';

export const WorkplaceWrapper = styled(Scrollbar)(() => ({
	width: '100%',
	height: '100%',
}));

export const SpaceLayoutWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'center',
	height: '100%',
	alignItems: 'center',
	width: 'fit-content',
	minWidth: '100%',
	minHeight: 'min-content',
	padding: `0 ${theme.spaces.m}px`,
}));
