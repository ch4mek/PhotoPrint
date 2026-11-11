import styled from '@emotion/styled';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';

export const AddMoreButtonWrapper = styled(ActionButton)(({ theme }) => ({
	color: theme.palette['text-accent'],
	width: 'auto',

	...theme.typography.body['m-400'],
}));
