import { getRequestStatusColor } from '@/helpers/getRequestStatusColor';

import { RequestStatusWrapper } from './styles';
import { RequestStatusProps } from './types';

export function RequestStatus({ status }: RequestStatusProps) {
	const backgroundColor = getRequestStatusColor(status);

	return (
		<RequestStatusWrapper {...{ backgroundColor }}>
			{status}
		</RequestStatusWrapper>
	);
}
