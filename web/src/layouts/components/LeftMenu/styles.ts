import styled from '@emotion/styled';
import { Sidebar } from '@mms/mms-ui-library/components/Sidebar';

const SIDEBAR_WRAPPER_MARGIN = 54;

export const SidebarWrapper = styled(Sidebar, {
	shouldForwardProp: (prop) => prop !== 'open',
})(() => ({
	marginTop: SIDEBAR_WRAPPER_MARGIN,
}));
