import { Typography } from '@mms/mms-ui-library/components/Typography';
import { PropsWithChildren } from 'react';

import { FormSectionWrapper } from './styles';
import { FormSectionProps } from './types';

export function FormSection({
	title,
	children,
	gap,
}: PropsWithChildren<FormSectionProps>) {
	return (
		<FormSectionWrapper {...{ gap }}>
			<Typography variant="m-600">{title}</Typography>
			{children}
		</FormSectionWrapper>
	);
}
