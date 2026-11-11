import { useTheme } from '@emotion/react';
import { Switch } from '@mms/mms-ui-library/components/Switch';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useFormikContext } from 'formik';
import { useCallback } from 'react';

import { TextArea } from '@/fields/TextArea';
import { PayloadFormValues } from '@/forms/PayloadForm/types';

import { NEED_A_PROJECT_TITLE, TEXT_AREAS_BASE_PROPS } from './constants';
import { FormContentWrapper, NeedProjectContainer } from './styles';

export function PaylaodDetails() {
	const theme = useTheme();

	const {
		values: { needProject },
		setFieldValue,
	} = useFormikContext<PayloadFormValues>();
	const handleSwitchChange = useCallback(() => {
		setFieldValue('needProject', !needProject);
	}, [needProject]);

	const textAreas = [
		{
			...TEXT_AREAS_BASE_PROPS.comment,
			onChange: setFieldValue,
		},
		{
			...TEXT_AREAS_BASE_PROPS.plan,
			onChange: setFieldValue,
		},
	];

	return (
		<FormContentWrapper>
			<NeedProjectContainer>
				<Switch checked={needProject} onChange={handleSwitchChange} />
				<Typography variant="s-400" color={theme.palette['text-gray']}>
					{NEED_A_PROJECT_TITLE}
				</Typography>
			</NeedProjectContainer>
			{textAreas.map(({ name, ...props }) => (
				<TextArea key={name} name={name} {...props} />
			))}
		</FormContentWrapper>
	);
}
