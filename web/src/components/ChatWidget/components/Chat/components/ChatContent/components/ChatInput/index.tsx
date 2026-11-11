import { useTheme } from '@emotion/react';
import { TextArea } from '@mms/mms-ui-library/components/TextArea';
import { SendIcon } from '@mms/mms-ui-library/icons/base/SendIcon';
import { Formik, FormikHelpers } from 'formik';
import { ChangeEvent, useCallback, useMemo, useRef } from 'react';

import { EVENT_ENTER_KEY } from '@/constants/eventKeys';
import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import dayjs from '@/lib/dayjs';
import { useAnalyzeQuestion } from '@/queries/digitalHr/useAnalyzeQuestion';
import { ChatFormValues, ChatRole, Message } from '@/types/chatWidget';

import {
	CHAT_INPUT_PLACEHOLDER,
	initialChatInputValue,
	MAX_FIELD_ROWS_COUNT,
	MAX_INPUT_VALUE_LENGTH,
	MESSAGE_FIELD,
	MIN_FIELD_ROWS_COUNT,
} from './constants';
import { validationSchema } from './schema';
import { AdornmentWrapper, TextAreaWrapper } from './styles';
import { ChatInputProps } from './types';

export function ChatInput({
	isLoading,
	addNewMessage,
	setIsMessageRequestLoading,
}: ChatInputProps) {
	const theme = useTheme();
	const toast = useToast();
	const fieldRef = useRef<HTMLDivElement>(null);

	const stylesConfig = useMemo(
		() => ({
			border: {
				borderColor: theme.palette['graphics-divider-1'],
			},
			textArea: {
				backgroundColor: 'transparent',
			},
		}),
		[theme]
	);

	const { mutateAsync, isPending } = useAnalyzeQuestion({
		onSuccess: (data) => addNewMessage(data, false),
		onError: createOnErrorHandler(toast),
		onSettled: () => setIsMessageRequestLoading(false),
	});

	const handleSubmit = useCallback(
		async (
			values: ChatFormValues,
			{ resetForm }: FormikHelpers<ChatFormValues>
		) => {
			const newMessageCreatedDateUtc = dayjs().utc().toISOString();

			const newMessage: Message = {
				id: newMessageCreatedDateUtc,
				createdAtUtc: newMessageCreatedDateUtc,
				message: values.question.trim(),
				role: ChatRole.User,
			};

			addNewMessage(newMessage, true);
			resetForm();
			setIsMessageRequestLoading(true);
			await mutateAsync(values);
		},
		[]
	);

	return (
		<Formik
			enableReinitialize
			initialValues={initialChatInputValue}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			validateOnBlur={false}
		>
			{({
				errors,
				touched,
				values,
				submitForm,
				setFieldValue,
				setFieldTouched,
			}) => {
				const isHasError = !!errors[MESSAGE_FIELD] && !!touched[MESSAGE_FIELD];
				const isValidField = !!(values[MESSAGE_FIELD].trim() && !isHasError);

				const handleChatInputChange = (e: ChangeEvent<HTMLTextAreaElement>) => {
					setFieldValue(MESSAGE_FIELD, e.target.value, true);
					setFieldTouched(MESSAGE_FIELD, true);
				};

				const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
					if (e.key === EVENT_ENTER_KEY && !e.shiftKey) {
						e.preventDefault();

						if (isValidField) {
							submitForm();
						}
					}
				};

				return (
					<TextAreaWrapper>
						<TextArea
							label=""
							ref={fieldRef}
							size="fullWidth"
							value={values[MESSAGE_FIELD]}
							onChange={handleChatInputChange}
							name={MESSAGE_FIELD}
							minRows={MIN_FIELD_ROWS_COUNT}
							maxRows={MAX_FIELD_ROWS_COUNT}
							isHighlightOnFocus={false}
							placeholder={CHAT_INPUT_PLACEHOLDER}
							stylesConfig={stylesConfig}
							isScrollVisible={false}
							disabled={isLoading || isPending}
							onKeyDown={handleKeyDown}
							maxLength={MAX_INPUT_VALUE_LENGTH}
							isAbleVoiceInput={false}
							endAdornment={
								<AdornmentWrapper
									onClick={submitForm}
									isActive={isValidField}
									disabled={!isValidField || isLoading || isPending}
								>
									<SendIcon />
								</AdornmentWrapper>
							}
						/>
					</TextAreaWrapper>
				);
			}}
		</Formik>
	);
}
