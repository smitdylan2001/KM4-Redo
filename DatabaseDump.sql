-- phpMyAdmin SQL Dump
-- version 4.9.4
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Generation Time: Jun 16, 2022 at 09:55 AM
-- Server version: 10.5.16-MariaDB
-- PHP Version: 7.4.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `dylansmit`
--

-- --------------------------------------------------------

--
-- Table structure for table `Score`
--

CREATE TABLE `Score` (
  `id` int(11) NOT NULL,
  `userID` int(11) NOT NULL,
  `score` int(11) NOT NULL,
  `dateTime` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `Score`
--

INSERT INTO `Score` (`id`, `userID`, `score`, `dateTime`) VALUES
(1, 1, 420, '2021-04-22 08:30:37'),
(2, 1, 21, '2021-04-22 08:30:37'),
(3, 1, 2345, '2021-04-22 08:30:37'),
(4, 1, 233, '2021-04-22 01:50:37'),
(5, 1, 2, '2021-04-22 04:35:37'),
(6, 1, 4212, '2021-04-22 02:30:37'),
(7, 17, 42340, '2021-05-22 08:30:37'),
(8, 17, 4720, '2021-04-22 08:23:37'),
(9, 17, 4260, '2021-04-22 08:40:37'),
(10, 17, 4420, '2021-04-22 08:23:37'),
(11, 17, 4204, '2021-04-22 18:30:37'),
(12, 18, 4420, '2021-04-22 08:31:37'),
(13, 18, 4260, '2021-04-22 08:32:37'),
(14, 18, 4270, '2021-04-22 08:32:38'),
(15, 18, 4820, '2021-04-22 08:33:37'),
(16, 18, 4320, '2021-04-22 08:34:37'),
(17, 19, 54, '2021-04-22 08:31:37'),
(18, 19, 4260, '2021-04-22 08:32:37'),
(19, 19, 4270, '2021-04-22 08:32:38'),
(20, 19, 4820, '2021-04-22 08:33:37'),
(21, 19, 4320, '2021-04-22 08:38:37'),
(22, 20, 432, '2021-04-12 08:31:37'),
(23, 20, 89, '2021-01-22 08:32:37'),
(24, 20, 4670, '2021-02-22 08:32:38'),
(25, 20, 2820, '2021-04-15 08:33:37'),
(26, 20, 4340, '2021-04-19 08:34:37'),
(27, 21, 332, '2021-04-02 08:31:37'),
(28, 21, 750, '2021-01-12 08:32:37'),
(29, 21, 640, '2021-02-22 08:32:38'),
(30, 21, 2222, '2020-03-15 08:33:37'),
(31, 21, 4540, '2021-04-16 08:34:37'),
(32, 1, 420, '2021-04-30 11:57:12'),
(33, 20, 500, '2021-04-30 12:03:01'),
(34, 1, 420, '2022-06-15 20:31:53'),
(35, 1, 420, '2022-06-15 20:31:53'),
(36, 1, 420, '2022-06-15 20:32:18'),
(37, 1, 420, '2022-06-15 20:32:18'),
(38, 1, 420, '2022-06-15 20:32:19'),
(39, 1, 420, '2022-06-15 20:32:19'),
(40, 1, 420, '2022-06-15 20:32:19'),
(41, 1, 420, '2022-06-15 20:32:19'),
(42, 1, 420, '2022-06-15 20:32:19'),
(43, 1, 420, '2022-06-15 20:32:19'),
(44, 1, 420, '2022-06-15 20:32:20'),
(45, 1, 420, '2022-06-15 20:32:20'),
(46, 1, 420, '2022-06-15 20:32:20'),
(47, 1, 420, '2022-06-15 20:32:20'),
(48, 17, 17, '2022-06-15 20:33:46'),
(49, 17, 17, '2022-06-15 20:33:46'),
(50, 17, 17, '2022-06-15 20:33:48'),
(51, 17, 17, '2022-06-15 20:33:48'),
(52, 17, 17, '2022-06-15 20:33:48'),
(53, 17, 17, '2022-06-15 20:33:48'),
(54, 17, 17, '2022-06-15 20:33:48'),
(55, 17, 17, '2022-06-15 20:33:48'),
(56, 4, 17, '2022-06-15 20:34:17'),
(57, 4, 17, '2022-06-15 20:34:17'),
(58, 4, 17, '2022-06-15 20:34:18'),
(59, 4, 17, '2022-06-15 20:34:18'),
(60, 4, 17, '2022-06-15 20:38:44'),
(61, 4, 17, '2022-06-15 20:38:45'),
(62, 4, 17, '2022-06-15 20:38:46'),
(63, 4, 17, '2022-06-15 20:38:46'),
(64, 4, 17, '2022-06-15 20:38:46'),
(65, 4, 17, '2022-06-15 20:38:46'),
(66, 4, 17, '2022-06-15 20:51:26'),
(67, 4, 17, '2022-06-15 20:51:26');

-- --------------------------------------------------------

--
-- Table structure for table `Servers`
--

CREATE TABLE `Servers` (
  `id` int(11) NOT NULL,
  `name` varchar(40) NOT NULL,
  `password` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `Servers`
--

INSERT INTO `Servers` (`id`, `name`, `password`) VALUES
(1, 'Server1', 'Admin1'),
(2, 'Server2', 'Admin2'),
(3, 'Server3', 'Admin3');

-- --------------------------------------------------------

--
-- Table structure for table `Users`
--

CREATE TABLE `Users` (
  `id` int(11) NOT NULL,
  `name` varchar(40) NOT NULL,
  `email` varchar(60) NOT NULL,
  `password` varchar(30) NOT NULL,
  `serverID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `Users`
--

INSERT INTO `Users` (`id`, `name`, `email`, `password`, `serverID`) VALUES
(1, 'awf', 'rw', 'strs', 1),
(17, 'Dylan', 'test@test.nl', 'testPass', 1),
(18, 'Carl', 'Carl@test.nl', 'testPassCarl', 1),
(19, 'Mark', 'mark@test.nl', 'testPassMark', 1),
(20, 'Bob', 'Bob@test.nl', 'testPassBob', 1),
(21, 'Dane', 'Dane@test.nl', 'testPassDane', 1),
(22, 'dyllo', 'dyl@fhfh.com', 'passwords', 1),
(23, 'SmitDylan', 'dyll@smit.com', 'newpassword', 1),
(24, 'SmitDylan', 'dylly@smit.com', 'newpassword', 1);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `Score`
--
ALTER TABLE `Score`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `Servers`
--
ALTER TABLE `Servers`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `Users`
--
ALTER TABLE `Users`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `Score`
--
ALTER TABLE `Score`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=78;

--
-- AUTO_INCREMENT for table `Servers`
--
ALTER TABLE `Servers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `Users`
--
ALTER TABLE `Users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
