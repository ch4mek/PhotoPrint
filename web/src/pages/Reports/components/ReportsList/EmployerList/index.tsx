import { SingleTab, Tabs } from '@mms/mms-ui-library/components/Tabs';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { useMemo, useState, memo, useEffect, useCallback } from 'react';

import { usePieChartSize } from '../hooks';

import { DetailedInfoContainer } from '@/components/DetailedInfoContainer';
import { DetailedInfoItem } from '@/components/DetailedInfoItem';
import { CustomPieChart } from '@/components/PieChart';
import { TotalTimeChartLabel } from '@/components/TotalTimeChartLabel';
import { convertToChartData } from '@/helpers/convertToChartData';

import { employerChartTypes } from './constants';
import {
	Wrapper,
	TabsWrapper,
	ChartName,
	SingleChartWrapper,
	PieChartWithInfoWrapper,
} from './styles';
import { EmployerListProps, ActiveChartIndexes } from './types';

const { desktopXS } = breakpoints;

function EmployerListComponent({
	chartData,
	generalDuration,
	isFetching,
}: EmployerListProps) {
	const [currentMobileChartType, setCurrentMobileChartType] = useState(0);

	const [activeChartIndexes, setActiveChartIndexes] =
		useState<ActiveChartIndexes>({});

	const isDesktopXSWidth = useHasScreenWidth(desktopXS);

	const displayedChartTypes = useMemo(() => {
		if (isDesktopXSWidth) {
			return employerChartTypes;
		}

		return [employerChartTypes[currentMobileChartType]];
	}, [isDesktopXSWidth, currentMobileChartType]);

	useEffect(() => {
		const updatedActiveChartIndexes = { ...activeChartIndexes };

		Object.keys(activeChartIndexes).forEach((chartType) => {
			if (!displayedChartTypes.includes(chartType)) {
				delete updatedActiveChartIndexes[chartType];
			}
		});

		setActiveChartIndexes(updatedActiveChartIndexes);
	}, [displayedChartTypes]);

	useEffect(() => {
		if (!isFetching) {
			setActiveChartIndexes({});
		}
	}, [isFetching]);

	const pieChartSize = usePieChartSize();

	const handleClickOnReportListItem = (index: number) => () => {
		setActiveChartIndexes({
			[employerChartTypes[currentMobileChartType]]: index,
		});
	};

	const handlePieEnter = useCallback(
		(chartType: string) => (index?: number) => {
			setActiveChartIndexes((prevValue) => ({
				...prevValue,
				[chartType]: index,
			}));
		},
		[]
	);

	const convertedChartData = useMemo(
		() =>
			convertToChartData(chartData[employerChartTypes[currentMobileChartType]]),
		[currentMobileChartType, chartData]
	);

	const chartTypeTabs = useMemo<SingleTab[]>(
		() =>
			employerChartTypes.map((type, index) => ({
				id: index,
				text: `Per ${type.split('per').join('')}`,
			})),
		[employerChartTypes]
	);

	return (
		<Wrapper>
			<TabsWrapper>
				<Tabs
					data={chartTypeTabs}
					activeTab={currentMobileChartType}
					onChangeTab={setCurrentMobileChartType}
				/>
			</TabsWrapper>
			<PieChartWithInfoWrapper>
				{!isFetching &&
					displayedChartTypes.map((chartType) => (
						<SingleChartWrapper key={chartType}>
							<ChartName variant="h3">per {chartType.split('per')}</ChartName>

							<CustomPieChart
								activeIndex={activeChartIndexes[chartType]}
								onPieEnter={handlePieEnter(chartType)}
								data={convertToChartData(chartData?.[chartType])}
								size={pieChartSize}
								showAdditionalInfo={isDesktopXSWidth}
							/>

							<TotalTimeChartLabel
								text={generalDuration[chartType]}
								isLabelStatic={isDesktopXSWidth}
							/>
						</SingleChartWrapper>
					))}

				<DetailedInfoContainer>
					{!isFetching &&
						convertedChartData.map((item, index) => (
							<DetailedInfoItem
								key={item.name}
								isActive={
									activeChartIndexes[
										employerChartTypes[currentMobileChartType]
									] === index
								}
								onClick={handleClickOnReportListItem(index)}
								{...item}
							/>
						))}
				</DetailedInfoContainer>
			</PieChartWithInfoWrapper>
		</Wrapper>
	);
}

export const EmployerList = memo(EmployerListComponent);
