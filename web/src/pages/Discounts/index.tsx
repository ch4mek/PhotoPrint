import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { useEffect, useState } from 'react';

import { MobileSidebar } from '@/components/MobileSidebar';
import { ControlButton } from '@/components/MobileSidebar/components/ControlButton';
import { discountBreakpoints } from '@/constants/discounts';
import { DiscountProvider } from '@/context/DiscountContext';
import { DiscountsCategorySection } from '@/pages/Discounts/components/DiscountsCategorySection';
import { PageWrapper } from '@/pages/styles';

import { DiscountsSection } from './components/DiscountsSection';
import { DiscountsSectionWrapper } from './styles';

export function DiscountsPage() {
	const [isSidebarOpen, setIsSideBarOpen] = useState(false);
	const isDesktopLWidth = useHasScreenWidth(discountBreakpoints.desktopL);

	const handleControlSideBar = () =>
		setIsSideBarOpen((prevState) => !prevState);
	const handleCloseSideBar = () => setIsSideBarOpen(false);

	useEffect(() => {
		if (isDesktopLWidth) {
			handleCloseSideBar();
		}
	}, [isDesktopLWidth]);

	return (
		<PageWrapper>
			<DiscountProvider>
				<DiscountsSectionWrapper>
					{!isDesktopLWidth ? (
						<>
							<ControlButton
								isOpen={isSidebarOpen}
								onClick={handleControlSideBar}
							/>
							<MobileSidebar
								handleControlSideBar={handleControlSideBar}
								isOpen={isSidebarOpen}
							>
								<DiscountsCategorySection />
							</MobileSidebar>
						</>
					) : (
						<DiscountsCategorySection />
					)}
					<DiscountsSection />
				</DiscountsSectionWrapper>
			</DiscountProvider>
		</PageWrapper>
	);
}
