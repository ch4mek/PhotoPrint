import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useMemo } from 'react';

import { getFullPhotoStyleConfig } from '../../constants';
import { PhotoItem } from '../NameWithPhotoItem/NameWithPhotoItem';

import { PopUpTitle } from '@/components/PopUpTitle';
import { useResize } from '@/hooks/useResize';

import { DescriptionWrapper, PopUpWrapper } from './styles';
import { PopupWithFullImageProps } from './types';
import { getPopUpStylesConfig } from './utils/getPopUpStylesConfig';

export function PopupWithFullImage({
	description,
	name,
	photoFileId,
	handleClose,
}: PopupWithFullImageProps) {
	const [, windowHeight] = useResize();
	const theme = useTheme();

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	const imageStyleConfig = getFullPhotoStyleConfig(theme);

	return (
		<PopUp
			title={<PopUpTitle headerText={name} />}
			onClose={handleClose}
			type="info"
			showCloseButton
			stylesConfig={popUpStylesConfig}
		>
			<PopUpWrapper>
				<PhotoItem styleConfig={imageStyleConfig} imageId={photoFileId} />
				<DescriptionWrapper>{description}</DescriptionWrapper>
			</PopUpWrapper>
		</PopUp>
	);
}
