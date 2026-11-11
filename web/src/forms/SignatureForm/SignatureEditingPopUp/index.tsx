import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useFormikContext } from 'formik';
import { useMemo } from 'react';

import { signatureFieldsRequired, signatureRightLabels } from '../constants';
import { SignatureFormValues, SignatureProps } from '../Signature/types';
import { SignatureFieldsWrapper } from '../styles';

import { TextField } from '@/fields/Text';
import { FormWithSaveExit } from '@/forms/components/FormWithSaveExit';
import { getTrimedString } from '@/helpers/getTrimedString';
import { useResize } from '@/hooks/useResize';

import { POPUP_TITLE } from './constants';
import { SignatureEditingPopUpProps } from './types';
import { getPopUpStylesConfig } from './utils';

export function SignatureEditingPopUp({
	onClose,
	onApply,
}: SignatureEditingPopUpProps) {
	const {
		values,
		touched,
		errors,
		dirty,
		setFieldValue,
		validateField,
		setFieldTouched,
	} = useFormikContext<SignatureProps>();
	const theme = useTheme();
	const [, windowHeight] = useResize();

	const handleChange = (fieldName: string, fieldValue: string) => {
		const filteredValue =
			fieldName === 'phone' ? fieldValue.replace(/[^\d+]/g, '') : fieldValue;

		setFieldValue(fieldName, filteredValue);
		validateField(fieldName);
		setFieldTouched(fieldName, true);
	};

	const handleOnApply = () => {
		Object.keys(values).forEach((field) =>
			setFieldValue(field, getTrimedString(values[field]))
		);
		onApply();
	};

	const isDisabled = (value: string) => ['website', 'email'].includes(value);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	return (
		<FormWithSaveExit isFormDirty={dirty} onClose={onClose}>
			{(handleOutsideClose) => (
				<PopUp
					title={POPUP_TITLE}
					onClose={onClose}
					onShadowClick={handleOutsideClose}
					type="form"
					controls={{
						negativeControl: {
							onClick: onClose,
						},
						positiveControl: { onClick: handleOnApply },
					}}
					stylesConfig={popUpStylesConfig}
				>
					<SignatureFieldsWrapper>
						{Object.keys(values).map((key) => (
							<TextField
								key={key}
								name={key}
								label={signatureRightLabels[key as keyof SignatureFormValues]}
								size="fullWidth"
								isRequired={
									signatureFieldsRequired[key as keyof SignatureFormValues]
								}
								disabled={isDisabled(key)}
								onChange={handleChange}
								error={Boolean(
									touched[key as keyof SignatureProps] &&
										errors[key as keyof SignatureProps]
								)}
							/>
						))}
					</SignatureFieldsWrapper>
				</PopUp>
			)}
		</FormWithSaveExit>
	);
}
