import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Tabs } from '@mms/mms-ui-library/components/Tabs';
import { useEffect, useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { useBookingMapContext } from '../../context/BookingMapContext';

import { mapTabsConfig, popUpStylesConfig } from './constants';
import { MapImageContainer } from './styled';

export function MapPopUp() {
	const [activeTab, setActiveTab] = useState<number>(mapTabsConfig[0].id);
	const {
		setIsMapOpen,
		setIsLoading,
		isLoading,
		setImageLoaded,
		isImageLoaded,
		resetImagesLoadStatus,
	} = useBookingMapContext();
	const theme = useTheme();
	const { t } = useTranslation('booking');

	const mapTabs = useMemo(
		() =>
			mapTabsConfig.map((tab) => ({
				...tab,
				text: t(tab.text),
			})),
		[t]
	);

	const currentMapImage = useMemo(
		() => mapTabsConfig.find((map) => map.id === activeTab)?.image[theme.mode],
		[theme, activeTab]
	);

	const currentImageKey = `${currentMapImage}-${theme.mode}`;

	useEffect(() => {
		setIsLoading(!isImageLoaded(currentImageKey));
	}, [currentImageKey, isImageLoaded, setIsLoading]);

	useEffect(() => () => resetImagesLoadStatus(), []);

	const handleMapClose = () => {
		setIsMapOpen(false);
	};

	const handleImageLoad = () => {
		setImageLoaded(currentImageKey);
		setIsLoading(false);
	};

	return (
		<PopUp
			title={t('map.officeMap')}
			onClose={handleMapClose}
			stylesConfig={popUpStylesConfig as any}
			type="info"
			showLoader={isLoading}
		>
			<Tabs onChangeTab={setActiveTab} data={mapTabs} activeTab={activeTab} />
			<MapImageContainer>
				<img
					src={currentMapImage}
					alt={t('map.officeMap')}
					onLoad={handleImageLoad}
				/>
			</MapImageContainer>
		</PopUp>
	);
}
