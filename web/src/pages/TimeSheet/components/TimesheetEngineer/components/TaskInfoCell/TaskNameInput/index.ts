import styled from '@emotion/styled';
import { Input } from '@mms/mms-ui-library/components/Input';

export const TaskNameInput = styled(Input)`
	& div:last-child {
		max-width: unset;
	}

	& input::placeholder {
		font-size: 12px;
	}
`;
