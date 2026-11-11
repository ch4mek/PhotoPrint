import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const ConfirmationTypography = styled(Typography)(
	({ theme }) => css`
		margin-top: ${theme.spaces.s}px;
	`
);
