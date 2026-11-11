import { Typography } from '@mms/mms-ui-library/components/Typography';

import { confirmationMessage } from '../DialogTypography/constants';
import { ConfirmationTypography } from '../DialogTypography/styles';

import { getActivationMessage } from '@/helpers/getActivationMessage';

import { ActivationMessageProps } from './types';

export function ActivationMessage({
	isActive,
	name,
	entityName,
}: ActivationMessageProps) {
	return (
		<>
			<Typography variant="m-400">
				<Typography variant="m-600">{name}</Typography>
				{getActivationMessage({
					isActive,
					entityName,
				})}
			</Typography>
			<ConfirmationTypography variant="m-400">
				{confirmationMessage}
			</ConfirmationTypography>
		</>
	);
}
