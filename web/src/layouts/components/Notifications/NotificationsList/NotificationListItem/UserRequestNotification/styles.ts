import styled from '@emotion/styled';

import { PreventOverflowTypography as StandardPreventOverflowTypography } from '../../styles';

export const PreventOverflowTypography = styled(
	StandardPreventOverflowTypography
)(({ theme }) => ({
	maxWidth: `calc(100% - ${theme.spaces.l + theme.spaces.xs}px)`,
}));
