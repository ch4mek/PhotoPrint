import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useEffect, useMemo, useRef, useState } from 'react';

import { PopUpTitle } from '@/components/PopUpTitle';
import { initialEditorData } from '@/constants/plateEditor';
import { useToast } from '@/context/Toast';
import { DescriptionSection } from '@/forms/Discount/DiscountViewForm/components/DescriptionSection';
import { DetailsDiscountInfo } from '@/forms/Discount/DiscountViewForm/components/DetailsDiscountInfo';
import { POPUP_CONTENT_HEIGHT } from '@/forms/Discount/DiscountViewForm/constants';
import { FormContent } from '@/forms/Discount/DiscountViewForm/styles';
import { getPopUpStylesConfig } from '@/forms/Discount/DiscountViewForm/utils/getStylesConfig';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useResize } from '@/hooks/useResize';
import { useGetDiscountById } from '@/queries/discounts/useGetDiscountById';

import { DiscountViewModalProps } from './types';

export function DiscountViewForm({
	discountId,
	handleClose,
}: DiscountViewModalProps) {
	const theme = useTheme();
	const toast = useToast();
	const [windowWidth, windowHeight] = useResize();

	const [infoHeight, setInfoHeight] = useState(POPUP_CONTENT_HEIGHT);
	const discountInfoRef = useRef<HTMLDivElement>(null);

	const {
		data: discountData,
		isLoading,
		isFetching,
	} = useGetDiscountById(discountId ?? '', {
		enabled: discountId != null,
		onError: (error) => {
			createOnErrorHandler(toast)(error);
			handleClose();
		},
	});

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, infoHeight, windowHeight, windowWidth),
		[theme, infoHeight, windowHeight, windowWidth]
	);

	const parsedEditorData = useMemo(() => {
		if (discountData?.text) {
			try {
				return JSON.parse(discountData.text);
			} catch {
				return initialEditorData;
			}
		}

		return initialEditorData;
	}, [discountData]);

	useEffect(() => {
		const el = discountInfoRef.current;

		if (!el) {
			return;
		}

		const observer = new ResizeObserver(() => {
			setInfoHeight(el.offsetHeight);
		});

		observer.observe(el);

		return () => observer.disconnect();
	}, []);

	return (
		<PopUp
			onClose={handleClose}
			title={<PopUpTitle headerText={discountData?.name || ''} />}
			type="form"
			showLoader={isLoading || isFetching}
			stylesConfig={popUpStylesConfig}
		>
			<FormContent>
				<DetailsDiscountInfo
					discountData={discountData}
					ref={discountInfoRef}
				/>
				<DescriptionSection
					discountData={discountData}
					initialData={parsedEditorData}
					initialText={discountData?.text}
				/>
			</FormContent>
		</PopUp>
	);
}
