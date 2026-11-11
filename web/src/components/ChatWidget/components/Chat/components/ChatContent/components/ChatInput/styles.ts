import styled from '@emotion/styled';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';

export const AdornmentWrapper = styled(ActionButton)({
	marginBottom: 'auto',
});

export const TextAreaWrapper = styled('div')(({ theme }) => ({
	marginBottom: 'auto',
	padding: `0 ${theme.spaces.m}px ${theme.spaces.m}px`,
}));
