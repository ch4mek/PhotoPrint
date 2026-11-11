import { Button } from '@mms/mms-ui-library/components/Button';
import { useCallback } from 'react';
import ReactDOMServer from 'react-dom/server';
import { useTranslation } from 'react-i18next';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import {
	COPY_TO_CLIPBOARD,
	HAS_BEEN_DOWNLOADED,
	NotificationType,
} from '@/constants/notifications';
import { Signature } from '@/forms/SignatureForm/Signature';
import { inlineImageSourcesInHtml } from '@/helpers/inlineImageSourcesInHtml';
import { triggerFileDownload } from '@/helpers/triggerFileDownload';

import { ButtonsWrapper } from './styles';
import type { SignatureControlsProps } from './types';

export function SignatureControls({
	isValid,
	values,
	setIsDialogOpen,
	showToast,
	showForm,
}: SignatureControlsProps) {
	const { t } = useTranslation('common', { keyPrefix: 'controlButtons' });
	const getHtmlSignature = async () => {
		const htmlSignature = ReactDOMServer.renderToStaticMarkup(
			<Signature {...values} mode="email" />
		);

		return htmlSignature;
	};

	const copyToClipboard = async () => {
		try {
			const htmlSignature = await getHtmlSignature();

			await navigator.clipboard.write([
				new ClipboardItem({
					'text/html': new Blob([htmlSignature], {
						type: 'text/html',
					}),
				}),
			]);

			showToast(COPY_TO_CLIPBOARD, NotificationType.SUCCESS);
		} catch {
			showToast(SOMETHING_WENT_WRONG, NotificationType.ERROR);
		}
	};

	const downloadHtmlFile = async () => {
		try {
			const htmlSignature = await getHtmlSignature();
			const htmlWithInlinedImages = await inlineImageSourcesInHtml(
				htmlSignature
			);
			const lowerCaseName = values.fullName.toLowerCase();
			const [firstInitial, lastName] = lowerCaseName.split(' ');
			const blob = new Blob([htmlWithInlinedImages], {
				type: 'text/html;charset=utf-8',
			});
			const fileName = `${firstInitial.charAt(0)}. ${lastName}.html`;

			triggerFileDownload(blob, fileName);
			showToast(HAS_BEEN_DOWNLOADED, NotificationType.SUCCESS);
		} catch {
			showToast(SOMETHING_WENT_WRONG, NotificationType.ERROR);
		}
	};

	const handleClearClick = useCallback(() => setIsDialogOpen(true), []);

	return (
		<ButtonsWrapper>
			<Button
				isFullWidth={false}
				color="secondary"
				disabled={!isValid}
				onClick={copyToClipboard}
			>
				{t('copy')}
			</Button>
			<Button
				isFullWidth={false}
				color="secondary"
				disabled={!isValid}
				onClick={downloadHtmlFile}
			>
				{t('download')}
			</Button>
			<Button isFullWidth={false} color="secondary" onClick={showForm}>
				{t('edit')}
			</Button>
			<Button isFullWidth={false} color="secondary" onClick={handleClearClick}>
				{t('clear')}
			</Button>
		</ButtonsWrapper>
	);
}
