import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback, useMemo } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { TextField } from '@/fields/Text';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import {
	initialFormValues,
	FORM_NAME,
} from '@/pages/Wiki/components/GroupSection/components/ArticleForm/constants';
import {
	ArticleFormProps,
	ArticleFormValues,
} from '@/pages/Wiki/components/GroupSection/components/ArticleForm/types';
import { useGetArticleById } from '@/queries/wiki/useGetArticlesById';
import { SegmentTypes } from '@/types/wiki';

import { validationSchema } from './schema';

export function ArticleForm({
	handleClose,
	handleSubmit,
	articleId,
}: ArticleFormProps) {
	const isEditMode = articleId != null;

	const { data, isLoading, isFetching } = useGetArticleById(articleId, {
		enabled: isEditMode,
	});

	const initialFormData = useMemo(() => {
		if (isEditMode && data) {
			return {
				name: data.name,
				keywords: data.keywords,
			};
		}

		return initialFormValues;
	}, [isEditMode, data]);

	const handleFormSubmit = useCallback(
		async (
			values: ArticleFormValues,
			{ setSubmitting }: FormikHelpers<ArticleFormValues>
		) => {
			try {
				setSubmitting(true);

				await handleSubmit({
					...(data || {}),
					...values,
				});
			} finally {
				setSubmitting(false);
			}
		},
		[handleSubmit, data]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialFormData}
			validationSchema={validationSchema}
			onSubmit={handleFormSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={FORM_NAME}
			id={articleId}
		>
			{({ submitForm, isSubmitting, setFieldValue, handleOutsideClose }) => (
				<PopUp
					type="form"
					showLoader={isSubmitting || isLoading || isFetching}
					title={
						isEditMode
							? getEditingPopUpTitle(SegmentTypes.Article)
							: getAddingPopUpTitle(SegmentTypes.Article)
					}
					onClose={handleClose}
					onShadowClick={handleOutsideClose}
					controls={{
						positiveControl: { onClick: submitForm },
						negativeControl: { onClick: handleClose },
					}}
				>
					<FormWrapper>
						<TextField
							name="name"
							label={`${SegmentTypes.Article} name`}
							onChange={setFieldValue}
							isRequired
						/>
						<TextField
							name="keywords"
							label="Keywords separated by spaces"
							onChange={setFieldValue}
							isRequired
						/>
					</FormWrapper>
				</PopUp>
			)}
		</FormikWithBackup>
	);
}
